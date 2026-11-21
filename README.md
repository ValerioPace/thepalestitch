# The Pale Stitch

Videogioco stealth fantasy-noir ambientato in una cittadina inglese medievale. Il protagonista è un ladro e assassino a pagamento che finisce invischiato in una catena di intrighi, fino allo scontro con un negromante. Il gioco ruota attorno a due cose: la luce in cui ci si nasconde e il rumore che si fa.

Versione corrente: **0.0.1** (graybox del prologo) + iterazione 1 in verifica. Motore: Godot 4.7.2 .NET (C#), rendering Forward+.

## Prerequisiti

| Strumento | Versione | Note |
|---|---|---|
| Godot Engine, edizione **.NET** | 4.7.2 | L'edizione standard non esegue C# |
| **.NET SDK** | 10 | L'SDK, non solo il runtime: lo richiede l'editor di Godot 4.7.2 per compilare. Da dotnet.microsoft.com |
| Connessione a internet | alla prima compilazione | Per scaricare i pacchetti NuGet di Godot |

## Avvio

1. Aprire `thepalestitch_project/project.godot` con Godot 4.7.2 .NET.
2. Compilare il C#: icona del martello in alto a destra.
3. Premere **F5**.

Per provare subito il livello senza passare da menu e video: aprire `thepalestitch_prologue.tscn` e premere **F6**.

Se il gioco resta fermo sulla prima schermata, il C# non è compilato: vedi [Technical Guide §27](docs/tech/TECHNICAL_GUIDE.md#27-diagnosi-dei-problemi).

## Comandi

| Azione | Tasto |
|---|---|
| Muoversi | WASD o frecce |
| Furtivo (attiva / disattiva) | C (o Ctrl) |
| Correre (tenere premuto) | Shift |
| Interagire | E |
| Ruotare la visuale | Q, R |
| Guardare in prima persona | V |
| Pausa | Esc |
| Saltare il video | Spazio, Invio, Esc |
| Pannello di debug (solo da editor) | F3 |

## Verifica

Prima di ogni commit, dalla radice del repository:

```powershell
powershell -ExecutionPolicy Bypass -File tools\verify.ps1
```

Compila, esegue i test unitari e gli scenari di gioco senza finestra, e termina con **VERIFY: GREEN** o **VERIFY: RED**. Se Godot non è in `X:\Installations\Godot_v4.7.2-stable_mono_win64\`, passare l'eseguibile *console* con `-Godot <percorso>`. Dettagli nella [Technical Guide §29](docs/tech/TECHNICAL_GUIDE.md#29-verifica-automatica).

## Documentazione

| Documento | Contenuto |
|---|---|
| [Technical Guide](docs/tech/TECHNICAL_GUIDE.md) | Fondamenti di Godot 4 .NET da zero, ogni sistema del gioco spiegato passo passo, ricette, diagnosi |
| [Architettura](docs/tech/ARCHITECTURE.md) | Moduli, dipendenze, flussi a runtime, configurazione, decisioni |
| [Piano dei task](docs/PLAN.md) | Iterazioni, ciclo di lavoro agentico, registro delle decisioni |
| [Backlog](docs/BACKLOG.md) | Difetti, change request e integrazioni nice to have, con priorità e stime |
| [Changelog](CHANGELOG.md) | Storia delle versioni |
| [Design: prologo, video, schermate](docs/design/01-prologo-video-schermate.md) | Premessa narrativa, flusso delle schermate, creazione del personaggio, prologo |

## Crediti

Carattere IM Fell English di Igino Marini, licenza SIL Open Font License (`thepalestitch_project/ui/fonts/OFL.txt`).
