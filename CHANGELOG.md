# Changelog

Tutte le modifiche rilevanti al progetto sono registrate in questo file.

Il formato segue [Keep a Changelog 1.1.0](https://keepachangelog.com/it-IT/1.1.0/) e il progetto adotta il [Versionamento Semantico](https://semver.org/lang/it/). Finché la versione principale è 0 il gioco è in sviluppo e ogni versione minore può cambiare qualsiasi cosa.

Ogni voce cita, quando c'è, l'Id del [Backlog](docs/BACKLOG.md).

Categorie: **Aggiunto** per nuove funzioni, **Modificato** per cambiamenti a funzioni esistenti, **Deprecato** per funzioni che verranno rimosse, **Rimosso**, **Corretto** per i difetti, **Sicurezza**.

## [Unreleased]

### Aggiunto
- **Verifica automatica** (CR-011 parte 1, CR-012): `tools/verify.ps1` e `tools/verify.sh` eseguono compilazione con avvisi trattati come errori, test unitari, importazione e scenari di gioco; esito VERIFY GREEN o RED con rapporto in `verify-report/`. `global.json` impone il .NET SDK 10.
- **Test unitari** (`tests/ThePaleStitch.Tests`, 44 casi): formule di furtività e personaggio, coerenza di layer, azioni di input, scene, interpolazione e testi localizzati.
- **Banco di scenari** (`dev/ScenarioRunner.tscn`, 11 scenari, 37 controlli): il gioco gira senza finestra a 144 fps simulati e verifica collisioni, udito della guardia, suggerimenti, furtivo, uscita al menu, cambi scena, fluidità e perimetro del prologo con la **mappa di raggiungibilità**.
- **Pannello di debug della furtività** (CR-020), tasto F3 nelle build di debug: coni e sospetto delle guardie, linea di vista, raggi di luce, valori numerici.
- Classi pure `Rules/StealthRules` e `Rules/CharacterRules` con le formule del gameplay; `Core/PhysicsLayers` con i layer fisici (CR-008).
- Andatura *Immobile* durante le azioni (testo "Immobile" nell'HUD).
- Documento di architettura del software (`docs/tech/ARCHITECTURE.md`): moduli, dipendenze, flussi a runtime, layer fisici, configurazione, decisioni architetturali.
- Technical Guide (`docs/tech/TECHNICAL_GUIDE.md`): fondamenti di sviluppo con Godot 4 .NET per chi non ha esperienza di videogiochi, spiegazione passo passo di tutti i sistemi del progetto, ricette operative, diagnosi, flusso di lavoro con git.
- Backlog (`docs/BACKLOG.md`) con 13 difetti, 20 change request e 9 integrazioni nice to have emersi dall'analisi della versione 0.0.1.
- Questo changelog.

### Modificato
- **Furtivo a interruttore** (CR-022): C o Ctrl attivano il furtivo, una seconda pressione lo disattiva. Shift tenuto premuto corre; al rilascio si torna all'andatura scelta (D-04).
- I suggerimenti del tutorial nominano i tasti tramite segnaposto riempiti con i tasti reali della Input Map (FIX-009); i testi nominano C prima di Ctrl.
- Rinominati `VariantDef`, `Variant()`, `VariantId` in `DyeDef`, `Dye()`, `DyeId` (FIX-012).
- **Prologo, perimetro del livello** (CR-021): chiusi i quattro varchi che permettevano di aggirare la ronda passando dietro le case o ai lati del cortile. Nuovi corpi `WestOutbuilding`, `AlleyEnd`, `LeanTo`; muri del cortile `YardWallWest` e `YardWallEast` alti 3,5 m. L'area raggiungibile passa da 2.713 a 382 m²; il vicolo resta come nascondiglio cieco.
- Documento di design: premessa narrativa confermata; nuovi principi di level design (§5.1).
- Piano dei task agentici (`docs/PLAN.md`) con iterazioni e registro delle decisioni.
- Backlog: nuove voci CR-022…CR-027 dal playtest del 21 novembre 2026 (furtivo a interruttore, percezione per parti del corpo, sguardi e visione periferica, allerta nel tempo, ronda adattiva, test di giocabilità automatici); CR-015 diventa un'epica.
- Documento di design: principio della furtività e scarti misurati rispetto al comportamento attuale.
- Backlog: CR-021 chiusa, aggiunta NTH-010 (saltare il prologo), CR-003 non più in attesa della premessa.
- README: prerequisiti (.NET SDK 10) e indice dei documenti.
### Corretto
- Giocatore e guardia non si attraversano più (FIX-001).
- Movimento fluido sui monitor oltre 60 Hz grazie all'interpolazione fisica (FIX-002).
- Un rumore non abbassa più il sospetto della guardia; la distanza si misura dalle orecchie (FIX-003).
- La campana non cancella più le istruzioni della serratura: notifiche e istruzioni sono due canali distinti (FIX-004).
- Durante la serratura la visibilità è neutra invece che ridotta come in furtivo (FIX-005, D-03).
- Formula del mascheramento sonoro in un solo punto; la barra del rumore mostra il raggio realmente emesso (FIX-006).
- I cambi scena richiesti durante una dissolvenza non vanno persi; gli errori vengono registrati (FIX-008).
- Animazioni dei suggerimenti non più sovrapposte (FIX-010).
- Dopo la cattura, uscire al menu prima della ripartenza non ricarica più il livello sopra il menu (FIX-014, trovato dal banco di scenari).

### Rimosso
- Codice inutilizzato: `GameState.HasProfile`, `SceneRouter.Boot` (FIX-011).

### Documentazione
- Technical Guide: interpolazione fisica (§5.5), andature e furtivo a interruttore, canali dei suggerimenti, nuovo capitolo §29 sulla verifica automatica. Architettura 1.1: namespace `Rules` e `Dev`, layer corretti, ADR-010…014.

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
