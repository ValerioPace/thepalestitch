# The Pale Stitch — Backlog

| | |
|---|---|
| Versione del documento | 1.0 |
| Riferito al software | 0.0.1, commit `b5b2eec` |
| Data | 21 novembre 2026 |
| Documenti collegati | [Architettura](tech/ARCHITECTURE.md) · [Technical Guide](tech/TECHNICAL_GUIDE.md) · [Changelog](../CHANGELOG.md) · [Design](design/01-prologo-video-schermate.md) |

Elenco unico di tutto il lavoro da fare sul software, nato dall'analisi della bozza 0.0.1. Le voci chiuse riportano data e verifica; le altre si riferiscono al codice del commit `b5b2eec`.

## Legenda

| Campo | Valori |
|---|---|
| **Tipo** | **FIX** difetto del codice esistente · **CR** change request: evoluzione necessaria per il gioco o per la qualità del software · **NTH** nice to have: integrazione desiderabile, non indispensabile |
| **Priorità** | **P1** blocca o falsa il gameplay, da fare subito · **P2** prossima iterazione · **P3** pianificabile · **P4** quando capita |
| **Stima** | **XS** < 1 h · **S** ≤ ½ giornata · **M** 1–3 giornate · **L** 1–2 settimane · **XL** oltre |
| **Stato** | Aperta · Decisione richiesta · In corso · Chiusa |

Le stime sono riferite al lavoro di sviluppo con il supporto di Claude, escluse produzione di asset artistici e audio.

## Riepilogo

| Id | Titolo | Tipo | Priorità | Stima | Stato |
|---|---|---|---|---|---|
| FIX-001 | Giocatore e guardia si attraversano | FIX | P1 | XS | Aperta |
| FIX-002 | Scatti visivi su monitor oltre 60 Hz | FIX | P2 | S | Aperta |
| FIX-003 | Un rumore può abbassare il sospetto della guardia | FIX | P2 | XS | Aperta |
| FIX-004 | La campana cancella le istruzioni della serratura | FIX | P2 | S | Aperta |
| FIX-005 | Bloccare l'input rende il personaggio meno visibile | FIX | P3 | S | Decisione richiesta |
| FIX-006 | Formula del mascheramento sonoro duplicata | FIX | P3 | XS | Aperta |
| FIX-007 | Rumori e raycast eseguiti fuori dal passo fisico | FIX | P3 | S | Aperta |
| FIX-008 | Cambi scena persi ed eccezioni silenziose | FIX | P3 | S | Aperta |
| FIX-009 | Nomi dei tasti scritti nei testi del tutorial | FIX | P3 | S | Aperta |
| FIX-010 | Animazioni dei suggerimenti sovrapposte | FIX | P4 | XS | Aperta |
| FIX-011 | Codice inutilizzato | FIX | P4 | XS | Aperta |
| FIX-012 | `CharacterProfile.Variant` oscura il tipo `Godot.Variant` | FIX | P4 | XS | Aperta |
| FIX-013 | Percorso della guardia ricalcolato a ogni passo fisico | FIX | P4 | S | Aperta |
| CR-001 | Edifici che coprono il personaggio | CR | P1 | M | Decisione richiesta |
| CR-002 | Suono che gira gli angoli e passa dalle porte | CR | P2 | L | Aperta |
| CR-003 | Interno della casa del mercante (prologo D–G) | CR | P1 | L | Aperta |
| CR-004 | Serratura come minigioco in prima persona | CR | P2 | M | Aperta |
| CR-005 | Uccisione silenziosa e trasporto dei corpi | CR | P2 | L | Aperta |
| CR-006 | Salvataggio e "Riprendi" | CR | P2 | M | Aperta |
| CR-007 | Sistema audio | CR | P2 | L | Aperta |
| CR-008 | Costanti uniche per i layer fisici | CR | P3 | S | Aperta |
| CR-009 | Catalogo del personaggio come risorse dell'editor, con validazione | CR | P3 | M | Aperta |
| CR-010 | Disaccoppiare HUD, giocatore e mondo | CR | P3 | M | Aperta |
| CR-011 | Test automatici e integrazione continua | CR | P2 | M | Aperta |
| CR-012 | Fissare la versione dell'SDK | CR | P3 | XS | Aperta |
| CR-013 | Gamepad e navigazione dei menu da tastiera | CR | P3 | M | Aperta |
| CR-014 | Livelli guidati da dati: trigger, obiettivi, eventi | CR | P2 | L | Aperta |
| CR-015 | IA delle guardie, seconda versione | CR | P2 | L | Aperta |
| CR-016 | Mesh di navigazione calcolata nell'editor | CR | P4 | XS | Aperta |
| CR-017 | Registro delle luci per il sensore | CR | P4 | S | Aperta |
| CR-018 | Doti dei mestieri e abilità Lama, Veleno, Nerbo | CR | P2 | L | Aperta |
| CR-019 | Modelli, animazioni e ritratti definitivi | CR | P2 | XL | Aperta |
| CR-020 | Pannello di debug della furtività | CR | P2 | S | Aperta |
| CR-021 | Chiudere i percorsi che aggirano la ronda del prologo | CR | P1 | S | Chiusa |
| NTH-001 | Menu opzioni | NTH | P3 | M | Aperta |
| NTH-002 | Accessibilità | NTH | P3 | M | Aperta |
| NTH-003 | Livelli di difficoltà | NTH | P4 | S | Aperta |
| NTH-004 | Il volto cambia le reazioni dei personaggi | NTH | P3 | M | Decisione richiesta |
| NTH-005 | Riepilogo della missione | NTH | P4 | S | Aperta |
| NTH-006 | Telemetria per i playtest | NTH | P4 | M | Aperta |
| NTH-007 | Altre lingue | NTH | P4 | S | Aperta |
| NTH-008 | Modalità foto | NTH | P4 | S | Aperta |
| NTH-009 | Integrazione con una piattaforma di distribuzione | NTH | P4 | M | Aperta |
| NTH-010 | Saltare il prologo per i giocatori esperti | NTH | P4 | S | Decisione richiesta |

## Ordine di lavoro consigliato

1. **Correttezza** (≈ 1 giornata): FIX-001, FIX-003, FIX-004, FIX-006, FIX-010, FIX-011, FIX-012.
2. **Fondamenta** (≈ 1 settimana): CR-012, CR-008, CR-011, CR-020, FIX-002, FIX-007, FIX-008. Da qui in poi ogni modifica è verificata da test e dal pannello di debug.
3. **Decisioni** sulle voci in stato *Decisione richiesta*.
4. **Contenuto del prologo**: CR-001, CR-003, CR-004, poi CR-005 e CR-018.
5. **Scalabilità**: CR-010, CR-014, CR-015, CR-002.

---

## FIX — Difetti

### FIX-001 · Giocatore e guardia si attraversano

| Tipo | Priorità | Stima | Componenti |
|---|---|---|---|
| FIX | P1 | XS | `scenes/player/Player.tscn`, `scenes/ai/Watchman.tscn` |

**Problema.** Il giocatore può camminare attraverso la guardia, e la guardia attraverso il giocatore.

**Causa.** Il giocatore sta sul layer 2 e ha maschera 1 (solo il mondo); la guardia sta sul layer 3 e ha maschera 1. Nessuno dei due ha l'altro nella propria maschera, quindi `MoveAndSlide` non li considera ostacoli.

**Proposta.** Maschera del giocatore = mondo + guardie (1 + 4 = **5**); maschera della guardia = mondo + giocatore (1 + 2 = **3**). Da valutare se la guardia debba spingere il giocatore o fermarsi.

**Criteri di accettazione.**
- Il giocatore non può sovrapporsi alla guardia da nessuna direzione.
- La guardia in ronda non attraversa il giocatore fermo.
- La guardia continua a vedere il giocatore "a contatto" (regola dei 1,3 m).

### FIX-002 · Scatti visivi su monitor oltre 60 Hz

| Tipo | Priorità | Stima | Componenti |
|---|---|---|---|
| FIX | P2 | S | `project.godot`, `CameraRig`, `PlayerController` |

**Problema.** Giocatore e guardia si muovono 60 volte al secondo (passo fisico), la camera li insegue a ogni frame disegnato. Su un monitor a 120 o 144 Hz alcuni frame mostrano il personaggio fermo e altri spostato di un passo intero: il movimento appare a scatti.

**Proposta.** Attivare l'interpolazione fisica (`physics/common/physics_interpolation = true`): il motore disegna i corpi in una posizione interpolata tra due passi fisici. Chiamare `ResetPhysicsInterpolation()` quando un nodo viene teletrasportato (ricomincio del livello). Verificare che la camera segua la posizione interpolata.

**Criteri di accettazione.** A 144 Hz, camminando in linea retta, nessuno scatto visibile del personaggio rispetto allo sfondo.

### FIX-003 · Un rumore può abbassare il sospetto della guardia

| Tipo | Priorità | Stima | Componenti |
|---|---|---|---|
| FIX | P2 | XS | `Watchman.OnNoise` |

**Problema.** `Suspicion = Min(0,9; Max(Suspicion, 0,4 + 0,3 × vicinanza))`. Se il sospetto era già 0,95, sentire un rumore lo porta a 0,9: un indizio in più rende la guardia **meno** sospettosa. Inoltre la vicinanza è calcolata dal piede della guardia, mentre la prova d'udito parte dagli occhi.

**Proposta.** `Suspicion = Max(Suspicion, Min(0,9; 0,4 + 0,3 × vicinanza))`, e usare la stessa posizione (gli occhi) in entrambi i calcoli.

**Criteri di accettazione.** Test unitario: con sospetto iniziale 0,95 un rumore vicino lascia il sospetto ≥ 0,95.

### FIX-004 · La campana cancella le istruzioni della serratura

| Tipo | Priorità | Stima | Componenti |
|---|---|---|---|
| FIX | P2 | S | `Hud.ShowHint`, `Hud.SetBell`, `PrologueLevel.UpdateBell` |

**Problema.** Le istruzioni della serratura sono un suggerimento permanente. Se la campana suona mentre si scassina, il suo suggerimento (6 s) le sostituisce e poi svanisce: il giocatore resta senza istruzioni.

**Proposta.** Due canali distinti nell'HUD: suggerimenti di **contesto** (permanenti, legati a un'azione in corso) e **notifiche** temporanee. In alternativa, una pila di suggerimenti con priorità che ripristina il precedente alla scadenza.

**Criteri di accettazione.** Scassinando durante un rintocco, alla fine della notifica della campana le istruzioni della serratura sono di nuovo visibili.

### FIX-005 · Bloccare l'input rende il personaggio meno visibile

| Tipo | Priorità | Stima | Componenti |
|---|---|---|---|
| FIX | P3 | S | `PlayerController._PhysicsProcess`, `UpdateVisibility` |

**Problema.** Quando `InputLocked` è vero (serratura, prima persona libera, cattura) l'andatura diventa *Furtivo* e la visibilità viene moltiplicata per 0,75. Scassinare rende quindi più difficili da vedere, ed è un effetto collaterale non dichiarato.

**Decisione richiesta.** Chi scassina dev'essere più visibile, meno visibile o neutro? Proposta: un'andatura esplicita *Al lavoro* con un proprio fattore (ad esempio 1,0) e un proprio rumore, invece di riusare *Furtivo*.

**Criteri di accettazione.** Il fattore di visibilità durante un'azione in prima persona è quello deciso e configurabile dall'Inspector.

### FIX-006 · Formula del mascheramento sonoro duplicata

| Tipo | Priorità | Stima | Componenti |
|---|---|---|---|
| FIX | P3 | XS | `NoiseBus.Emit`, `PlayerController.UpdateFootsteps` |

**Problema.** La riduzione dovuta al rumore ambientale (`1 − 0,6 × AmbientMask`) è scritta sia in `NoiseBus` sia in `PlayerController`, con il valore 0,6 ripetuto. Cambiandone uno solo, la barra del rumore dell'HUD smetterebbe di corrispondere a ciò che sentono le guardie.

**Proposta.** Un metodo pubblico `NoiseBus.Masked(float radius)` usato da entrambi; `Emit` restituisce il raggio effettivamente emesso.

**Criteri di accettazione.** Il valore 0,6 compare in un solo punto del codice.

### FIX-007 · Rumori e raycast eseguiti fuori dal passo fisico

| Tipo | Priorità | Stima | Componenti |
|---|---|---|---|
| FIX | P3 | S | `PrologueLevel.UpdateLock`, `NoiseBus`, `Watchman.OnNoise` |

**Problema.** I rumori della serratura sono emessi da `_Process`. Gli ascoltatori reagiscono subito con dei raycast, quindi le interrogazioni fisiche avvengono fuori da `_PhysicsProcess`. Oggi funziona perché la fisica gira sul thread principale; si romperebbe attivando la fisica su un thread separato, e rende il comportamento dipendente dagli FPS.

**Proposta.** Spostare la logica della serratura in `_PhysicsProcess`; in generale, far accodare i rumori a `NoiseBus` e consegnarli all'inizio del passo fisico successivo.

**Criteri di accettazione.** Nessun raycast eseguito fuori da `_PhysicsProcess` (verificabile con un controllo in debug).

### FIX-008 · Cambi scena persi ed eccezioni silenziose

| Tipo | Priorità | Stima | Componenti |
|---|---|---|---|
| FIX | P3 | S | `SceneRouter.GoTo` |

**Problema.** (1) Una richiesta di cambio scena durante una dissolvenza viene scartata senza avvisi. (2) Il metodo è `async void`: un'eccezione al suo interno non arriva a nessuno e può lasciare `_busy` a vero, bloccando ogni cambio scena successivo. (3) Se il caricamento fallisce, la vecchia scena ricompare senza spiegazioni.

**Proposta.** Ricordare l'ultima richiesta ed eseguirla al termine della transizione in corso; `try/finally` per riportare sempre `_busy` a falso; registrare gli errori con `GD.PushError`.

**Criteri di accettazione.** Due chiamate ravvicinate portano alla seconda destinazione; un percorso inesistente produce un errore nel pannello Output e il gioco resta utilizzabile.

### FIX-009 · Nomi dei tasti scritti nei testi del tutorial

| Tipo | Priorità | Stima | Componenti |
|---|---|---|---|
| FIX | P3 | S | `strings.csv` (`HINT_MOVE`, `HINT_LOOK`, `HINT_LOCKPICK`), `Hud` |

**Problema.** I suggerimenti dicono "WASD", "Ctrl", "E", "S", "V" in chiaro. Il prompt d'interazione invece legge il tasto reale dalla Input Map. Con tasti rimappati o con un gamepad i suggerimenti mentirebbero.

**Proposta.** Segnaposto nei testi (`{interact}`, `{move}`) sostituiti dall'HUD con `Hud.KeyLabel`.

**Criteri di accettazione.** Cambiando il tasto di `interact` nella Input Map, il suggerimento della serratura mostra il nuovo tasto.

### FIX-010 · Animazioni dei suggerimenti sovrapposte

| Tipo | Priorità | Stima | Componenti |
|---|---|---|---|
| FIX | P4 | XS | `Hud.FadeHint` |

**Problema.** Ogni comparsa o scomparsa di un suggerimento crea un nuovo tween sulla stessa proprietà senza fermare il precedente. Due chiamate ravvicinate producono un'opacità contesa.

**Proposta.** Conservare il tween in un campo e chiamare `Kill()` prima di crearne uno nuovo.

### FIX-011 · Codice inutilizzato

| Tipo | Priorità | Stima | Componenti |
|---|---|---|---|
| FIX | P4 | XS | `GameState.HasProfile`, `SceneRouter.Boot` |

**Problema.** `HasProfile` viene scritto ma mai letto; la costante `SceneRouter.Boot` non è mai usata. (I valori `NoiseKind.Impact` e `Voice` sono inutilizzati ma previsti da CR-005 e CR-015: restano.)

**Proposta.** Rimuoverli, oppure usare `HasProfile` per avvisare quando il prologo viene avviato senza personaggio (F6 dall'editor).

### FIX-012 · `CharacterProfile.Variant` oscura il tipo `Godot.Variant`

| Tipo | Priorità | Stima | Componenti |
|---|---|---|---|
| FIX | P4 | XS | `CharacterProfile`, `CharacterCreation`, `PlayerController` |

**Problema.** Dentro `CharacterProfile` il nome `Variant` indica la proprietà della tinta, non più il tipo `Godot.Variant` del motore. Oggi compila, ma il primo uso di `Variant` come tipo in quella classe darebbe un errore poco comprensibile, e chi legge confonde i due concetti.

**Proposta.** Rinominare la proprietà in `Dye` e il record in `DyeDef` (anche le chiavi `VARIANT_` possono restare).

### FIX-013 · Percorso della guardia ricalcolato a ogni passo fisico

| Tipo | Priorità | Stima | Componenti |
|---|---|---|---|
| FIX | P4 | S | `Watchman.StepTowards` |

**Problema.** `_agent.TargetPosition = target` viene assegnato 60 volte al secondo; il `NavigationAgent3D` ricalcola il percorso a ogni assegnazione. Con una guardia è irrilevante, con venti diventa un costo.

**Proposta.** Assegnare la meta solo quando cambia di più di 0,5 m.

---

## CR — Change request

### CR-001 · Edifici che coprono il personaggio

| Tipo | Priorità | Stima | Stato |
|---|---|---|---|
| CR | P1 | M | Decisione richiesta |

**Contesto.** Nella vista isometrica le case alte nascondono il personaggio nei vicoli. Oggi l'unico rimedio è ruotare la camera. In un gioco stealth vedere il proprio personaggio è essenziale.

**Opzioni.**
1. **Sagoma attraverso i muri:** il personaggio, quando è coperto, si vede come silhouette colorata (secondo passaggio di rendering senza test di profondità).
2. **Muri che svaniscono:** gli edifici tra camera e personaggio diventano trasparenti o vengono tagliati con un cerchio attorno al giocatore (shader con posizione del giocatore).
3. **Tetti che si scoperchiano:** entrando in un edificio, il suo tetto e la facciata verso la camera spariscono.

Le opzioni 2 e 3 sono complementari (la 3 serve comunque per gli interni del mercante, CR-003).

**Criteri di accettazione.** In nessun punto del prologo il giocatore perde di vista il proprio personaggio senza ruotare la camera.

### CR-002 · Suono che gira gli angoli e passa dalle porte

| Tipo | Priorità | Stima |
|---|---|---|
| CR | P2 | L |

**Contesto.** Il suono oggi viaggia in linea retta e ogni muro lo attenua del 55 %. Non esiste il concetto di porta aperta, e un rumore dietro l'angolo di un vicolo aperto è attenuato come se ci fosse un muro.

**Proposta.** Calcolare la distanza **lungo la mesh di navigazione** (il percorso che il suono farebbe nell'aria) e confrontarla con il raggio; trattare porte e finestre come *portali* con un proprio fattore di passaggio (aperta 1,0, chiusa 0,3). Mantenere il metodo attuale come approssimazione per i rumori lontani.

**Criteri di accettazione.** Un passo dietro l'angolo di un vicolo aperto si sente quanto all'aperto; una porta che si apre aumenta il raggio udibile nella stanza accanto.

### CR-003 · Interno della casa del mercante (prologo D–G)

| Tipo | Priorità | Stima | Stato |
|---|---|---|---|
| CR | P1 | L | Aperta |

**Contesto.** Il graybox finisce alla serratura. Il design prevede: scale che scricchiolano, buco della serratura, il mercante già morto con la bocca cucita, il cadavere che gira la testa, la trappola della ronda, la fuga dai tetti.

**Dipendenze.** CR-001 (opzione 3) per gli interni, CR-004 per la serratura, una prima versione dell'arrampicata per la fuga.

**Premessa narrativa** confermata il 21 novembre 2026 (vedi il [documento di design](design/01-prologo-video-schermate.md)). Resta da scegliere la soluzione di CR-001 per gli interni.

### CR-004 · Serratura come minigioco in prima persona

| Tipo | Priorità | Stima |
|---|---|---|
| CR | P2 | M |

**Contesto.** Oggi scassinare è "tieni premuto E". La prima persona esiste per le azioni che contano: dovrebbero chiedere abilità al giocatore.

**Proposta.** Minigioco a perni: muovendo il mouse si cerca il punto in cui il perno cede; ogni errore produce un rumore di tipo `Tool`. L'abilità Dita allarga la zona giusta e riduce il rumore degli errori.

### CR-005 · Uccisione silenziosa e trasporto dei corpi

| Tipo | Priorità | Stima |
|---|---|---|
| CR | P2 | L |

**Proposta.** Interazione alle spalle di una guardia ignara → passaggio in prima persona → uccisione con durata e rumore dipendenti da Lama. Il corpo diventa un oggetto trasportabile (Nerbo riduce la lentezza) e un **indizio**: una guardia che vede un corpo passa direttamente all'allarme o a una ricerca. Richiede `NoiseKind.Impact` e una prima versione della CR-015.

### CR-006 · Salvataggio e "Riprendi"

| Tipo | Priorità | Stima |
|---|---|---|
| CR | P2 | M |

**Proposta.** Salvataggio in `user://saves/` in formato JSON con numero di versione: profilo del personaggio, livello, punto di controllo, stato degli oggetti persistenti. Punti di controllo all'inizio di ogni sezione; nessun salvataggio libero durante le infiltrazioni (scelta di design da confermare). Il pulsante "Riprendi" del menu si attiva se esiste un salvataggio.

**Criteri di accettazione.** Chiudere il gioco dopo la creazione del personaggio e riaprirlo permette di riprendere con lo stesso personaggio; un file di salvataggio di una versione precedente viene migrato o rifiutato con un messaggio.

### CR-007 · Sistema audio

| Tipo | Priorità | Stima |
|---|---|---|
| CR | P2 | L |

**Proposta.** Bus audio (Master, Musica, Effetti, Voce, Ambiente); passi con suono diverso per superficie collegati allo stesso evento di `NoiseBus`; campana, voce narrante del video (inglese e italiano), musica del menu. Il suono reale e il rumore di gioco devono restare coerenti: un passo più forte deve anche **sentirsi** più forte.

### CR-008 · Costanti uniche per i layer fisici

| Tipo | Priorità | Stima |
|---|---|---|
| CR | P3 | S |

**Contesto.** I numeri dei layer (1, 2, 4, 8, 16) sono scritti nelle scene e in `NoiseBus.WorldMask`. Un errore, come quello di FIX-001, è difficile da vedere.

**Proposta.** Classe statica `PhysicsLayers` con costanti nominate (`World`, `Player`, `Guards`, `Interactables`, `Surfaces`) e maschere composte; un test che confronta i valori con `[layer_names]` di `project.godot`; layer e maschere dei nodi principali impostati da codice in `_Ready`, o almeno verificati.

### CR-009 · Catalogo del personaggio come risorse dell'editor, con validazione

| Tipo | Priorità | Stima |
|---|---|---|
| CR | P3 | M |

**Proposta.** Trasformare `TradeDef`, `CostumeDef`, `VariantDef`, `VisageDef` in classi `Resource` con `[GlobalClass]` e proprietà `[Export]`: ogni mestiere e costume diventa un file `.tres` modificabile dall'Inspector senza programmare. Validazione all'avvio: sei abilità tra 1 e 5, somma 16, Id univoci, chiavi presenti nel CSV, almeno una tinta per costume. Un Id sconosciuto nel profilo deve produrre un errore, non un ripiego silenzioso.

### CR-010 · Disaccoppiare HUD, giocatore e mondo

| Tipo | Priorità | Stima |
|---|---|---|
| CR | P3 | M |

**Contesto.** L'HUD interroga giocatore e guardie a ogni frame; `PlayerController` e `SurfaceZone` si conoscono a vicenda (dipendenza circolare tra i namespace `Player` e `World`).

**Proposta.**
- Un bus di eventi di gioco (`GameEvents`: visibilità cambiata, sospetto cambiato, allarme, obiettivo) a cui l'HUD si iscrive.
- Interfacce piccole al posto dei tipi concreti: `ISurfaceSink` implementata dal giocatore, `IPerceivable` per tutto ciò che una guardia può vedere (giocatore, corpi, porte aperte).
- Obiettivo: nessun ciclo tra namespace, HUD dipendente solo dagli eventi.

### CR-011 · Test automatici e integrazione continua

| Tipo | Priorità | Stima |
|---|---|---|
| CR | P2 | M |

**Proposta.**
1. **Test unitari** (xUnit) in un progetto separato per la logica pura: formule di `CharacterProfile`, mascheramento di `NoiseBus`, validazione del catalogo, aggiornamento del sospetto (estratto in una funzione pura).
2. **Test di scena** con GdUnit4 per C#: il giocatore non attraversa i muri, la guardia entra in indagine a un rumore vicino, il cambio di lingua aggiorna la creazione personaggio.
3. **Smoke test** senza finestra: avvio di ogni scena principale per alcuni secondi con controllo degli errori nell'output.
4. **CI** su GitHub Actions: compilazione, test, smoke test a ogni push su `develop` e a ogni pull request.

**Criteri di accettazione.** Una pull request che rompe la compilazione o un test viene segnalata in rosso.

### CR-012 · Fissare la versione dell'SDK

| Tipo | Priorità | Stima |
|---|---|---|
| CR | P3 | XS |

**Proposta.** Un file `global.json` in `thepalestitch_project/` che richiede l'SDK 10 (`"rollForward": "latestFeature"`): chi clona il progetto riceve un errore chiaro invece del problema incontrato all'installazione. Aggiornare il README con i prerequisiti (fatto in questa consegna).

### CR-013 · Gamepad e navigazione dei menu da tastiera

| Tipo | Priorità | Stima |
|---|---|---|
| CR | P3 | M |

**Proposta.** Associare a ogni azione anche i comandi del gamepad; movimento analogico (oggi `GetVector` lo supporta già); gestione del focus in tutti i menu, in particolare nella creazione personaggio dove i pulsanti sono generati da codice; icone dei tasti nell'HUD in base all'ultimo dispositivo usato.

### CR-014 · Livelli guidati da dati: trigger, obiettivi, eventi

| Tipo | Priorità | Stima |
|---|---|---|
| CR | P2 | L |

**Contesto.** `PrologueLevel` contiene logica valida solo per quel livello: soglie sulla coordinata X per il tutorial, campana, serratura, fine livello. Ogni nuovo livello richiederebbe una classe nuova.

**Proposta.** Componenti riusabili configurati nell'editor: `TutorialTrigger` (area che mostra un suggerimento), `ObjectiveTracker`, `AmbientNoiseSource` (la campana come nodo con inviluppo configurabile), `LevelExit`. La classe del livello si riduce a collegare questi componenti, o sparisce.

### CR-015 · IA delle guardie, seconda versione

| Tipo | Priorità | Stima |
|---|---|---|
| CR | P2 | L |

**Proposta.**
- Stati in più: *Ricerca* (dopo aver perso di vista il giocatore, controlla i nascondigli vicini), *Ritorno* alla ronda, *Allerta* (dopo un'indagine resta più attenta per un po').
- Comunicazione tra guardie: chi dà l'allarme lo propaga a quelle che lo sentono.
- Filtro dei rumori per tipo e sorgente: i passi di un'altra guardia non devono far indagare.
- Ronde con tempi d'attesa e direzioni dello sguardo per ogni tappa.
- Separare la logica in una macchina a stati esplicita (una classe per stato) testabile.

### CR-016 · Mesh di navigazione calcolata nell'editor

| Tipo | Priorità | Stima |
|---|---|---|
| CR | P4 | XS |

**Proposta.** Calcolare la mesh con il pulsante *Bake NavigationMesh* dell'editor e salvarla nella scena, togliendo il calcolo da `PrologueLevel._Ready`. Conveniente quando i livelli diventeranno grandi; finché si lavora in graybox il calcolo all'avvio è più comodo.

### CR-017 · Registro delle luci per il sensore

| Tipo | Priorità | Stima |
|---|---|---|
| CR | P4 | S |

**Proposta.** Le luci si registrano in un elenco quando entrano nell'albero (`_EnterTree`) e si cancellano quando escono. Il sensore considera solo quelle il cui raggio contiene il personaggio (griglia spaziale), invece di scorrere tutto il gruppo `stealth_light` tre volte per misura.

### CR-018 · Doti dei mestieri e abilità Lama, Veleno, Nerbo

| Tipo | Priorità | Stima |
|---|---|---|
| CR | P2 | L |

**Contesto.** Le quattro doti e tre abilità su sei sono oggi solo testo.

**Proposta.** Implementare ciascuna dove nasce la meccanica: Lama con CR-005, Nerbo con arrampicata e trasporto, Veleno con un primo oggetto consumabile (sonnifero) e l'avvelenamento di cibi e bevande. Ogni dote diventa un modificatore dichiarato nel catalogo.

### CR-019 · Modelli, animazioni e ritratti definitivi

| Tipo | Priorità | Stima |
|---|---|---|
| CR | P2 | XL |

**Proposta.** Pipeline degli asset: modelli in formato glTF importati in Godot, scheletro comune per protagonista e guardie, animazioni di camminata, furtivo, corsa, interazione. Tinte dei costumi applicate da shader (maschere di colore) invece che con materiali separati. Ritratti illustrati che rispettino la "regola del volto" del documento di design. `Mannequin.Apply` resta il punto d'aggancio.

### CR-020 · Pannello di debug della furtività

| Tipo | Priorità | Stima |
|---|---|---|
| CR | P2 | S |

**Proposta.** Con F3 (solo nelle build di debug): valori numerici di esposizione per ciascun punto di campionamento, raggi di rumore, cono visivo e sospetto di ogni guardia disegnati nel mondo, raycast di luce e vista come linee colorate. Indispensabile per tarare luce e suono, e il modo più rapido per **vedere** gli algoritmi della Technical Guide al lavoro.

### CR-021 · Chiudere i percorsi che aggirano la ronda del prologo

| Tipo | Priorità | Stima | Stato |
|---|---|---|---|
| CR | P1 | S | **Chiusa** il 21 novembre 2026 |

**Richiesta.** Nessuna scorciatoia: il giocatore non deve poter aggirare il cortile e la ronda per arrivare alla serratura. Si chiude con muri, e le aree escluse non si costruiscono (principi di level design, §5.1 del documento di design).

**Analisi.** Calcolando la mesh di navigazione per un agente delle dimensioni del giocatore e cercando i percorsi dal punto di partenza, risultavano raggiungibili 2.713 m²: quasi tutto il terreno. Varchi trovati:
1. estremità ovest della strada, tra il muro di testa e la prima casa;
2. fondo del vicolo, aperto verso il retro delle case;
3. fessura di 2 m tra la casa centrale e quella a est;
4. fianchi del cortile, chiusi solo da muretti corti di 1,6 m che lasciavano passare ai lati.

**Soluzione.** In `thepalestitch_prologue.tscn`, sotto `World`: tre nuovi corpi (`WestOutbuilding`, `AlleyEnd`, `LeanTo`) e due muri del cortile ridisegnati alti 3,5 m (`YardWallWest` da 0,5 × 5 m a 2 × 8 m, `YardWallEast` da 0,5 × 5 m a 2 × 13 m, fino al muro di testa est).

**Verifica.**
- Area raggiungibile dopo la modifica: 382 m², solo strada, vicolo cieco e cortile, con il cortile raggiungibile solo dal cancello.
- Nel gioco: spingendo il personaggio verso il vecchio varco del cortile, scivola lungo la facciata e non passa.
- Serratura, ronda della guardia e tutorial invariati; compilazione senza errori né avvisi.

---

## NTH — Nice to have

| Id | Titolo | Descrizione | Priorità | Stima |
|---|---|---|---|---|
| NTH-001 | Menu opzioni | Volume per bus, risoluzione e modalità finestra, rimappatura dei tasti, lingua, sensibilità del mouse in prima persona | P3 | M |
| NTH-002 | Accessibilità | Dimensione e sfondo dei sottotitoli, HUD con colori distinguibili anche da daltonici, riduzione del tremolio delle luci, opzione per attivare con una pressione invece di tenere premuto (furtivo, scassinare) | P3 | M |
| NTH-003 | Livelli di difficoltà | Moltiplicatori su vista e udito delle guardie, velocità del sospetto, aiuti visivi (anelli di rumore) disattivabili | P4 | S |
| NTH-004 | Il volto cambia le reazioni | Ad esempio il Marchiato viene riconosciuto dalle guardie, l'Occhio di latte spaventa i popolani. **Decisione richiesta:** il volto deve restare solo estetico? | P3 | M |
| NTH-005 | Riepilogo della missione | A fine livello: volte in cui si è stati visti, rumore prodotto, tempo, oggetti rubati | P4 | S |
| NTH-006 | Telemetria per i playtest | Registrazione locale di posizioni, rilevamenti e morti; mappe di calore per capire dove i giocatori vengono scoperti | P4 | M |
| NTH-007 | Altre lingue | Francese, tedesco, spagnolo; il pulsante della lingua diventa una lista | P4 | S |
| NTH-008 | Modalità foto | Camera libera a gioco fermo, filtri in stile incisione | P4 | S |
| NTH-009 | Piattaforma di distribuzione | Obiettivi e salvataggi nel cloud, ad esempio con Steam | P4 | M |
| NTH-010 | Saltare il prologo | Il prologo è il tutorial per i principianti; un giocatore esperto potrebbe saltarlo per intero, partendo dal capitolo 1 con un personaggio già creato. **Decisione richiesta** più avanti, quando il prologo sarà completo | P4 | S |

---

## Decisioni aperte

Voci che non possono partire senza una scelta del committente.

| Voce | Domanda |
|---|---|
| CR-001 | Quale soluzione per gli edifici che coprono il personaggio: sagoma, muri trasparenti, tetti che si scoperchiano, o una combinazione? |
| FIX-005 | Durante un'azione in prima persona il personaggio dev'essere più visibile, meno visibile o neutro? |
| NTH-004 | Il volto scelto resta solo estetico o cambia le reazioni dei personaggi? |
| CR-006 | Salvataggio libero o solo a punti di controllo? |
| NTH-010 | Il giocatore esperto può saltare il prologo? (da decidere a prologo completo) |
