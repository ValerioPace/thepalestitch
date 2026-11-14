# The Pale Stitch

Videogioco stealth fantasy-noir ambientato in una cittadina inglese medievale. Il protagonista è un ladro e assassino a pagamento che finisce invischiato in una catena di intrighi, fino allo scontro con un negromante. Il gioco ruota attorno a due cose: la luce in cui ci si nasconde e il rumore che si fa.

Motore: Godot 4.7.2 .NET (C#), rendering Forward+.

## Avvio

1. Aprire `thepalestitch_project/project.godot` con Godot 4.7.2 (versione .NET).
2. Compilare la soluzione C# (icona del martello in alto a destra, o `dotnet build` nella cartella del progetto).
3. Premere F5.

Per provare subito il livello senza passare dal menu: aprire `thepalestitch_prologue.tscn` e premere F6.

## Cosa c'è

- Schermata d'avvio, video iniziale in-engine, menu principale.
- Creazione del personaggio in tre passi: mestiere, veste, volto.
- Prologo in graybox: una strada, una guardia, la porta del mercante di lana.
- Prima versione dei sistemi di luce e ombra, propagazione del suono e sensi delle guardie.
- Testi in inglese e italiano.

## Comandi

| Azione | Tasto |
|---|---|
| Muoversi | WASD o frecce |
| Furtivo | Ctrl (o C) |
| Correre | Shift |
| Interagire | E |
| Ruotare la visuale | Q, R |
| Guardare in prima persona | V |
| Pausa | Esc |

## Documenti

- [Prologo, video iniziale e schermate](docs/design/01-prologo-video-schermate.md)

## Crediti

Carattere IM Fell English di Igino Marini, licenza SIL Open Font License (`thepalestitch_project/ui/fonts/OFL.txt`).
