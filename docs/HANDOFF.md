# The Pale Stitch — Nota di ripresa

Ultimo aggiornamento: 21 novembre 2026. Da leggere **per primo** all'inizio di ogni sessione di lavoro di Claude, poi aggiornare a fine sessione.

## Dove siamo

| | |
|---|---|
| Repository | `X:\Projects\thepalestitch` (sul device dell'utente; remote `github.com/ValerioPace/thepalestitch`) |
| Branch di lavoro | `feature/iterazione-1`, creato da `feature/prologue` (`ba2a633`); **non pushato**, merge a cura del committente dopo il playtest (D-02) |
| Commit | `af2be33` codice (T-1.1, 1.2, 1.4, 1.5) · `7580d8d` verifica automatica (T-1.3) · ultimo commit `docs:` (documentazione e decisioni) |
| Iterazione 1 | Completata e consegnata: tutte le voci *In verifica* nel Backlog. Il committente sta facendo playtest |
| Iterazione 2 | **Approvata, si parte la settimana del 23 novembre 2026.** Decisioni D-06…D-10 prese con le raccomandazioni del Piano |
| Verifica | `tools\verify.ps1` (Windows) / `tools/verify.sh`: VERIFY GREEN, 44 test, 37 controlli in 11 scenari |

## Prossima azione

1. Chiedere al committente l'esito del playtest dell'iterazione 1. Osservazioni → correzioni sul branch `feature/iterazione-1`; se accettata, le voci *In verifica* diventano *Chiuse* e si rilascia `0.1.0` (dopo il suo merge).
2. Aprire `feature/iterazione-2` dal punto concordato (da `feature/iterazione-1` se non ancora mergiato, altrimenti da `develop`).
3. Eseguire l'iterazione 2 secondo [PLAN.md](PLAN.md) §3, **iniziando da T-2.1** (banco di giocabilità, CR-027), lanciato anche sull'IA attuale per avere i numeri "prima". Il primo percorso negligente da far fallire è quello del playtest: fermo, in piedi e in luce a ~2 m di fianco alla guardia durante la campana.
4. Ordine: T-2.1 → T-2.2 (CR-023, regola D-05) → T-2.3 (CR-024, CR-025) → T-2.4 (CR-026, FIX-013) → T-2.5 (FIX-007 e taratura). Soglie D-09: 80 % / 10 % su 50 semi.

## Regole di lavoro concordate

- **Gameplay:** "difficile, non impossibile, nessuna stupidità degli avversari". D-05: busto in luce nel cono → allarme istantaneo (game over); spalla, braccio, gamba in luce → forte sospetto, tempo di reazione, brusche girate di testa verso vicoli e nascondigli. Nessuna scorciatoia di level design: perimetri chiusi da muri, verificati con la mappa di raggiungibilità.
- **Comandi:** furtivo a interruttore con C/Ctrl; corsa con Shift tenuto, al rilascio torna l'andatura scelta (D-04); visibilità neutra durante le azioni (D-03).
- **Processo:** ciclo e definizione di "fatto" del Piano §1. Verify verde prima di ogni consegna; documentazione (Technical Guide, Architettura, Backlog, Changelog, README, Piano) aggiornata nello stesso task e copiata anche nel Project claude.ai (`claude/PLAN.md`, `claude/BACKLOG.md`, `claude/CHANGELOG.md`, `claude/HANDOFF.md`, `tech/*`, `design/*`).
- **Scene:** modifiche puntuali ai `.tscn`, mai rigenerati per intero.
- **Commit:** su branch dedicati, sempre compilabili, Id del backlog nel messaggio, identità dell'ultimo autore del repository (`git -c user.name=… -c user.email=…`), mai push. Permesso di cancellazione nella cartella del repository da richiedere a inizio sessione (git crea e rimuove file di blocco).
- **Lingua:** documentazione in italiano; commenti nel codice in inglese (XML doc).

## Note tecniche per l'ambiente di Claude

- Lavorare su una copia nel container, verificare, poi copiare sul device i soli file cambiati controllando prima `git status` e gli md5 del device (l'utente può aver modificato scene nell'editor).
- Nel container nuget.org è bloccato: i test usano il runner `tests/ThePaleStitch.Tests/Framework/MiniTest.cs` in stile xUnit (ADR-011).
- Godot nel container: `godot --headless --fixed-fps 144 --path thepalestitch_project res://dev/ScenarioRunner.tscn -- --report=<dir>`; il verify fallisce anche su ogni riga `ERROR`/`WARNING` del log.
- Sul PC dell'utente: .NET SDK 10 (10.0.404), Godot in `X:\Installations\Godot_v4.7.2-stable_mono_win64\`.
