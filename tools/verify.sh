#!/usr/bin/env bash
# Full verification of The Pale Stitch: build, unit tests, gameplay scenarios.
# Usage: tools/verify.sh            (GODOT=/path/to/godot to choose the engine)
# Exit code 0 only if every step is green. Report in verify-report/.
set -u
ROOT="$(cd "$(dirname "$0")/.." && pwd)"
GODOT="${GODOT:-godot}"
REPORT="$ROOT/verify-report"
mkdir -p "$REPORT"
FAILED=0
step() { printf '\n== %s\n' "$1"; }

step "1/4 Build (warnings are errors)"
if dotnet build "$ROOT/thepalestitch_project/The Pale Stitch.csproj" -warnaserror -nologo -v q > "$REPORT/build.log" 2>&1; then
  echo "OK"; else echo "FAILED (see $REPORT/build.log)"; FAILED=1; fi

step "2/4 Unit tests"
if dotnet run --project "$ROOT/tests/ThePaleStitch.Tests" > "$REPORT/unit-tests.log" 2>&1; then
  tail -1 "$REPORT/unit-tests.log"; else grep -E "FAILED|Unit tests" "$REPORT/unit-tests.log"; FAILED=1; fi

step "3/4 Import resources"
"$GODOT" --headless --path "$ROOT/thepalestitch_project" --import > "$REPORT/import.log" 2>&1
echo "done"

step "4/4 Gameplay scenarios (headless, simulated time)"
"$GODOT" --headless --fixed-fps 144 --path "$ROOT/thepalestitch_project" res://dev/ScenarioRunner.tscn -- --report="$REPORT" > "$REPORT/scenarios.log" 2>&1
CODE=$?
grep -E "FAIL|Scenarios:" "$REPORT/scenarios.log"
NOISE=$(grep -cE "^(ERROR|WARNING|SCRIPT ERROR)" "$REPORT/scenarios.log")
if [ "$CODE" -ne 0 ]; then FAILED=1; fi
if [ "$NOISE" -ne 0 ]; then echo "Engine errors/warnings in the log: $NOISE"; grep -E "^(ERROR|WARNING|SCRIPT ERROR)" "$REPORT/scenarios.log" | sort | uniq -c; FAILED=1; fi

echo
if [ "$FAILED" -eq 0 ]; then echo "VERIFY: GREEN"; else echo "VERIFY: RED"; fi
exit $FAILED
