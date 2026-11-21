# Changelog

Tutte le modifiche rilevanti al progetto sono registrate in questo file.

Il formato segue [Keep a Changelog 1.1.0](https://keepachangelog.com/it-IT/1.1.0/) e il progetto adotta il [Versionamento Semantico](https://semver.org/lang/it/). Finché la versione principale è 0 il gioco è in sviluppo e ogni versione minore può cambiare qualsiasi cosa.

Ogni voce cita, quando c'è, l'Id del [Backlog](docs/BACKLOG.md).

Categorie: **Aggiunto** per nuove funzioni, **Modificato** per cambiamenti a funzioni esistenti, **Deprecato** per funzioni che verranno rimosse, **Rimosso**, **Corretto** per i difetti, **Sicurezza**.

## [Unreleased]

### Aggiunto
- Documento di architettura del software (`docs/tech/ARCHITECTURE.md`): moduli, dipendenze, flussi a runtime, layer fisici, configurazione, decisioni architetturali.
- Technical Guide (`docs/tech/TECHNICAL_GUIDE.md`): fondamenti di sviluppo con Godot 4 .NET per chi non ha esperienza di videogiochi, spiegazione passo passo di tutti i sistemi del progetto, ricette operative, diagnosi, flusso di lavoro con git.
- Backlog (`docs/BACKLOG.md`) con 13 difetti, 20 change request e 9 integrazioni nice to have emersi dall'analisi della versione 0.0.1.
- Questo changelog.

### Modificato
- README: prerequisiti (.NET SDK 10) e indice dei documenti.
- `project.godot` riformattato dall'editor di Godot all'apertura del progetto: stessa configurazione, formattazione diversa delle azioni di input; la riga `locale/fallback="en"` è stata rimossa perché uguale al valore predefinito.

## [0.0.1] — 2026-11-14

Prima bozza giocabile (graybox). Commit `b5b2eec` *feat: prima stesura bozza prologo*.

### Aggiunto
- **Progetto**: Godot 4.7.2 .NET, C#, rendering Forward+, fisica Jolt; soluzione `The Pale Stitch.sln` e progetto `The Pale Stitch.csproj` (namespace radice `ThePaleStitch`).
- **Infrastruttura**: autoload `GameState` (personaggio, lingua, video visto, impostazioni in `user://settings.cfg`) e `SceneRouter` (cambi scena con dissolvenza al nero); azioni di input centralizzate in `InputActions`.
- **Schermate**: avvio con avvertimento; video iniziale in-engine di 5 inquadrature con narrazione a sottotitoli e titolo cucito (74 s, saltabile, mostrato solo al primo avvio); menu principale con cambio lingua; creazione del personaggio in tre passi (mestiere, veste con tinta, volto e soprannome); HUD; menu di pausa.
- **Personaggio**: 4 mestieri su 6 abilità, 4 costumi con 3 tinte ciascuno, 6 volti, 6 soprannomi; effetti su rumore, visibilità e tempo di scasso.
- **Furtività**: sensore di luce con raycast verso le luci del gruppo `stealth_light`; bus dei rumori con attenuazione dei muri e mascheramento ambientale; 5 superfici con rumorosità diverse; anelli di rumore disegnati a terra.
- **Camera**: vista isometrica ortografica con rotazione a scatti di 90°; prima persona libera e d'azione.
- **IA**: guardia con lanterna, ronda, indagine sui rumori e su ciò che vede, allarme; navigazione su mesh calcolata all'avvio del livello.
- **Prologo**: strada notturna con torce, ronda, campana dell'abbazia, serratura della casa del mercante; tutorial contestuale; fine del graybox alla serratura aperta, ricomincio in caso di allarme.
- **Interfaccia**: tema unico `pale_theme.tres`, font IM Fell English (licenza OFL), shader di nebbia per i menu e di vignettatura per la prima persona, controlli disegnati (occhio della luce, punti abilità, cucitura, ritratto).
- **Localizzazione**: 128 chiavi in inglese e italiano.
- **Documentazione**: documento di design del prologo, del video e delle schermate (`docs/design/01-prologo-video-schermate.md`).

### Problemi noti
- Vedi [Backlog](docs/BACKLOG.md), sezione FIX.

[Unreleased]: https://github.com/ValerioPace/thepalestitch/compare/b5b2eec...develop
[0.0.1]: https://github.com/ValerioPace/thepalestitch/commit/b5b2eec
