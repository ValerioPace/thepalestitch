# The Pale Stitch — Prologo, video iniziale e schermate

Documento di design n. 1. Descrive ciò che è stato impostato nel progetto Godot e ciò che resta sulla carta.
Dove una voce è marcata **[proposta]** è un'idea narrativa da confermare, non una decisione presa.

Stato del progetto: Godot 4.7.2 .NET, C#, rendering Forward+, mondo 3D con camera isometrica ortografica. Testi in inglese e italiano.

---

## 1. Premessa narrativa

**Confermata il 21 novembre 2026.** I capitoli successivi la approfondiranno, anche in modo interattivo, in base alle scelte del protagonista e alle abilità che affina.

| Elemento | Proposta |
|---|---|
| Luogo | **Harrowmere**, cittadina inglese di mercanti di lana, ai margini delle paludi |
| Anno | 1352, tre inverni dopo la grande pestilenza: fosse piene, chiese vuote |
| Protagonista | Ladro e assassino a pagamento. Senza nome: il giocatore sceglie solo il soprannome di strada |
| Committenti | I signorotti locali e i loro intendenti, che pagano in argento e non fanno domande |
| Il "punto pallido" | I morti cominciano a comparire con la bocca cucita da un filo chiaro come l'osso. È la firma del Negromante: il punto impedisce all'anima di uscire, e il corpo si rialza |
| Gancio personale | Il protagonista porta un punto identico sotto il colletto, da una notte che non ricorda |
| Arco | Lineare: dagli incarichi dei signorotti alla scoperta di chi cuce i morti, fino allo scontro con il Negromante evocatore di demoni |

Il titolo acquista così un doppio senso: è il marchio dell'antagonista ed è la cicatrice del protagonista.

---

## 2. Flusso delle schermate

```
Avvio (Boot) ──► Video iniziale ──► Menu principale ──► Creazione personaggio ──► Prologo
                  (solo al primo avvio;                    I. Mestiere
                   poi si rivede dal menu)                 II. Veste
                                                           III. Volto
```

| Schermata | Scena | Stato |
|---|---|---|
| Avvio | `scenes/boot/Boot.tscn` | Fatta: riga di avvertimento su nero, poi prosegue da sola |
| Video iniziale | `scenes/intro/IntroCinematic.tscn` | Fatta in graybox: 5 inquadrature + titolo, saltabile |
| Menu principale | `scenes/menu/MainMenu.tscn` | Fatta. "Riprendi" è presente ma disattivato (manca il salvataggio) |
| Creazione personaggio | `scenes/character/CharacterCreation.tscn` | Fatta e funzionante, con anteprime segnaposto |
| Prologo | `thepalestitch_prologue.tscn` | Graybox giocabile fino alla porta del mercante |
| HUD e pausa | `scenes/ui/Hud.tscn` | Fatti |

Ogni cambio scena passa da `SceneRouter` (dissolvenza al nero). Il personaggio scelto vive in `GameState`.

### Direzione visiva dell'interfaccia

- Fondo quasi nero con nebbia lenta (shader), mai immagini luminose.
- Tre colori soltanto: osso (`#d9d2c0`) per il testo, sangue rappreso (`#6e1414`) per la selezione, ottone spento per le righe "di sapore".
- Carattere IM Fell English (licenza OFL, incluso in `ui/fonts`): un tondo, un corsivo, un maiuscoletto.
- Il motivo ricorrente è la **cucitura**: sotto il titolo, nei punti delle abilità (croci di filo), nella sequenza finale del video.
- Tutto lo stile sta in un solo file: `ui/theme/pale_theme.tres`.

---

## 3. Video iniziale

Realizzato in-engine: nessun file video. La scena contiene un set graybox e un nodo `Shots` con un figlio per inquadratura. Ogni inquadratura ha quattro marcatori che si spostano dall'editor: `CamFrom`, `CamTo` (percorso della camera), `LookFrom`, `LookTo` (punto guardato). Durata, campo visivo e riga di narrazione sono proprietà dell'inquadratura.

Durata totale: circa 74 secondi. Voce narrante: il protagonista, in prima persona.

| # | Inquadratura | Durata | Narrazione (IT) |
|---|---|---|---|
| 1 | Una candela, un tavolo, un ago su un telo da sudario. Cartello: *Harrowmere, Anno Domini 1352* | 13 s | Tre inverni dalla pestilenza. Le fosse sono piene, le chiese vuote, e i vivi hanno imparato a guardare altrove. |
| 2 | Lenta discesa sui tetti della città nella nebbia, poche torce | 13 s | Una città così ha bisogno di uomini come me. Qualcuno deve pur aprire le porte chiuse a chiave, e chiudere gli occhi che hanno visto troppo. |
| 3 | La forca sulla collina, dal basso | 13 s | Ho derubato i suoi signori e ho ucciso per loro. Pagano in argento e pagano puntuali. Non ho mai chiesto di chi fosse la mano che reggeva la borsa. |
| 4 | Un corpo sotto un sudario in un vicolo. La camera scende fino alla bocca cucita | 14 s | Poi i morti hanno cominciato a farsi trovare con la bocca cucita. Un lavoro fino. Punti piccoli, regolari, con un filo pallido come l'osso. |
| 5 | In soggettiva verso la porta di una cripta, luce verde malata dall'interno | 13 s | Conosco quel punto. Ne porto uno uguale sotto il colletto, da una notte che non riesco a ricordare. |
| — | Nero. Una ferita si cuce punto dopo punto, poi appare il titolo | 8 s | — |

Da produrre: voce narrante (EN e IT), musica, suoni (l'ago che passa nella tela in apertura, campana, corvi), modelli definitivi al posto dei blocchi.

---

## 4. Creazione del personaggio — "Chi regge l'ago"

Il protagonista è sempre ladro e assassino. Il giocatore non sceglie *cosa* è, ma in cosa è più bravo, cosa indossa e che faccia ha. Tre passi, nessun cursore, nessun punto da distribuire.

### I. Mestiere

Sei abilità da 1 a 5. Ogni mestiere ne ha 16 punti in tutto.

| Mestiere | Passo | Ombra | Dita | Lama | Veleno | Nerbo | Dote |
|---|---|---|---|---|---|---|---|
| Il Tagliaborse | 4 | 3 | 5 | 1 | 1 | 2 | *Mano leggera*: serrature più rapide e silenziose |
| Lo Strangolatore | 3 | 3 | 1 | 5 | 1 | 3 | *Lavoro silenzioso*: uccisioni alle spalle mute, corpi più facili da trascinare |
| L'Avvelenatore | 2 | 3 | 3 | 1 | 5 | 2 | *Peccato dello speziale*: parte con sonnifero e veleno lento |
| Il Topo di Tetto | 3 | 4 | 2 | 2 | 1 | 4 | *Gronde e cornicioni*: si arrampica dove gli altri non possono |

Cosa fanno le abilità:

| Abilità | Effetto | Nel codice oggi |
|---|---|---|
| Passo | Raggio del rumore dei passi | Sì: ×1,17 a Passo 1, ×0,85 a Passo 5 |
| Ombra | Quanto si è visibili nella penombra | Sì: ×1,13 a Ombra 1, ×0,85 a Ombra 5 |
| Dita | Tempo per aprire una serratura | Sì: 3,9 s a Dita 1, 1,5 s a Dita 5 |
| Lama | Uccisioni rapide e silenziose | Non ancora |
| Veleno | Veleni e sonniferi | Non ancora |
| Nerbo | Arrampicata, trasporto dei corpi | Non ancora |

Le doti sono per ora solo testo.

### II. Veste

Ogni veste cambia tre numeri e ha tre tinte (lo "skinning"). La tinta è solo estetica.

| Veste | Visibilità | Rumore | Tasche | Travestimento | Tinte |
|---|---|---|---|---|---|
| Mantello del Boia | −20% | +10% | 3 | La gente si scosta e non guarda in faccia | Pece, Sangue rappreso, Muschio di fossa |
| Cuoio del Conciatore | 0% | +25% | 6 | L'odore tiene tutti a distanza | Catrame, Fiele di bue, Pelle cruda |
| Stracci del Becchino | +15% | −20% | 4 | Nessuno ferma chi porta i morti | Calce viva, Cenere, Argilla di fossa |
| Saio del Falso Frate | −5% | 0% | 5 | Le porte dell'abbazia si aprono | Fuliggine, Muffa, Vino acido |

Visibilità e rumore agiscono già sul gioco. Tasche e travestimento sono dati pronti per sistemi futuri (inventario, reazioni dei passanti).

### III. Volto e soprannome

| Volto | Descrizione |
|---|---|
| Butterato | La pestilenza l'ha preso e rimesso giù, tenendosi la pelle della faccia |
| Segnato dal cappio | Impiccato una volta, con una corda vecchia. Il segno è rimasto |
| Occhio di latte | Un occhio bianco, bruciato da un coltello rovente |
| Marchiato | La lettera dei ladri impressa a fuoco sulla guancia |
| Labbro spaccato | Una lama gli ha aperto la bocca fino ai denti |
| Scavato | Tre inverni di fame: guarda la gente come un corvo guarda un agnello |

Soprannomi: Ago, Corvo, Sego, Acchiapparatti, Stoppino, Pendaglio.

### La regola del volto

Nessun personaggio selezionabile può avere tratti delicati o virtuosi. Per ogni ritratto, modello o variante futura vale questa lista:

1. Il viso è lungo e duro: mascella, zigomi, mai tondo.
2. Gli occhi sono in ombra, stretti. Mai grandi, mai limpidi.
3. Le sopracciglia scendono verso il naso. Nessuna espressione aperta o serena.
4. Ogni volto porta un **danno visibile**: malattia, cicatrice, marchio, fame.
5. Metà del viso è sempre persa nel buio del cappuccio.
6. La pelle è terrea o grigia, mai rosea.
7. La postura è curva, il capo avanti.
8. Niente simboli di virtù: nessuna croce portata con devozione, nessun colore chiaro e pulito. Il saio è rubato, il lino è sporco di calce.

I ritratti attuali sono disegnati dal codice (`PortraitCard.cs`) e servono a fissare questa direzione finché non esistono le illustrazioni.

---

## 5. Prologo — "Un lavoro pulito"

### Sequenza completa (design)

L'intendente di un signorotto affida un incarico semplice: entrare di notte nella casa di un mercante di lana, prendere il suo registro, e fare in modo che non possa parlarne. **[proposta]**

| # | Momento | Cosa insegna | Stato |
|---|---|---|---|
| A | La strada. Torce sui muri, una guardia con la lanterna che fa la ronda | Luce e ombra, andature | Graybox giocabile |
| B | Paglia, ghiaia, fango, assi. La campana dell'abbazia copre i passi quando suona | Rumore, superfici, mascheramento | Graybox giocabile |
| C | La porta del mercante: la visuale entra in prima persona per forzare la serratura | Passaggio in prima persona | Giocabile (serratura semplificata) |
| D | Interno: scale che scricchiolano, uno sguardo dal buco della serratura | Prima persona per osservare | Da fare |
| E | La stanza: il mercante è già morto sulla sedia, con la bocca cucita di filo pallido. Preso il registro, la testa del cadavere si gira | **Prima sorpresa** | Da fare |
| F | La candela si spegne, la campana suona a martello, la ronda batte alla porta gridando all'assassino. Qualcuno ti ha mandato lì per farti trovare | **Seconda sorpresa**: l'incarico era una trappola | Da fare |
| G | Fuga dai tetti. Taglio sul titolo del primo capitolo | Arrampicata | Da fare |

### Il livello graybox

Una strada da ovest a est. A nord case alte che gettano ombra sulla strada; a sud un parapetto basso, così la vista isometrica resta aperta. In fondo, dietro un cortile, la casa del mercante.

- **Luci**: tre torce (bocca del vicolo, metà strada, cancello) e la luna. Metà strada è in ombra, metà al chiaro di luna.
- **Superfici**: paglia vicino al carro, ghiaia a metà strada e nel cortile, una passerella di assi fino alla porta, fango nel vicolo.
- **Ripari**: carro, casse, botti, due vicoli bui.
- **Guardia**: un uomo della ronda con lanterna, su un giro di quattro punti (`PatrolRound`, spostabili).
- **Campana**: ogni 32 secondi suona per 7. Finché suona i passi portano fino al 60% in meno.

- **Perimetro chiuso**: la sola area calpestabile è la strada, il vicolo (cieco, un nascondiglio) e il cortile del mercante, raggiungibile solo dal cancello. Le fessure tra le case e i fianchi del cortile sono murati (vedi §5.1).

Farsi vedere fa ricominciare la scena. Aprire la serratura chiude il graybox e riporta al menu.

Il prologo è anche il **tutorial per i principianti**. Se permettere ai giocatori esperti di saltarlo per intero si deciderà più avanti (NTH-010 nel backlog).

### 5.1 Principi di level design

1. **Nessuna scorciatoia che aggiri la sfida.** Ogni livello ha un percorso pensato attorno alla sua prova (qui: passare sotto la ronda). Un giro alternativo che la evita non è ammesso, per quanto astuto. Se un percorso alternativo esiste, è perché è stato progettato come tale, con la sua sfida.
2. **Il perimetro si chiude con muri, non con barriere invisibili.** Il giocatore deve capire dove può andare guardando il mondo.
3. **Ciò che sta oltre il perimetro non si costruisce.** Le aree irraggiungibili restano vuote: nessun dettaglio, nessuna luce, nessun costo di produzione o di rendering.
4. **Il perimetro si verifica, non si presume.** Prima di chiudere un livello si calcola la mappa dell'area raggiungibile dal punto di partenza e la si confronta con quella progettata.

Applicazione al prologo: la verifica sulla versione 0.0.1 ha mostrato che dal punto di partenza si raggiungevano 2.713 m², cioè quasi tutto il terreno, comprese le aree dietro le case e i fianchi del cortile. Dopo la chiusura di quattro varchi (estremità ovest, fondo del vicolo, fessura tra la casa centrale e quella a est, fianchi del cortile) l'area raggiungibile è di 382 m²: strada, vicolo e cortile.

---

## 6. I due sistemi centrali

### Luce e ombra (`LightSensor.cs`)

Il sensore misura quanto è illuminato il personaggio, da 0 a 1, in tre punti (stinchi, petto, testa).

- Contano le luci nel gruppo **`stealth_light`**. Per aggiungere una luce al gioco basta metterla in quel gruppo.
- Una luce conta solo se tra lei e il personaggio non c'è un muro: l'ombra di una casa è un nascondiglio vero.
- Torce e lanterne calano con la distanza: `energia × (1 − distanza/raggio)² × 0,5`.
- La luna vale `energia × 0,5` se il cielo è libero, zero se c'è un tetto o un muro di mezzo.
- C'è sempre un minimo di 0,04.

Visibilità finale = esposizione × veste × Ombra × andatura (furtivo 0,75, al passo 1, di corsa 1,15).

Nota: la luce di gioco è separata da quella a schermo. Il livello è reso più chiaro di quanto "valga" (luce ambiente blu, più una piccola luce sul personaggio che non conta per la furtività) perché il giocatore deve sempre vedere dov'è.

### Propagazione del suono (`NoiseBus.cs`)

Ogni rumore è una sfera con un raggio.

| Andatura | Raggio base | Passo ogni |
|---|---|---|
| Furtivo | 1,5 m | 0,62 s |
| Al passo | 4,5 m | 0,45 s |
| Di corsa | 9 m | 0,30 s |

| Superficie | Moltiplicatore |
|---|---|
| Paglia | ×0,45 |
| Fango | ×0,70 |
| Selciato | ×1,00 |
| Assi | ×1,25 |
| Ghiaia | ×1,60 |

Raggio del passo = base × superficie × veste × Passo, poi ridotto dal rumore d'ambiente (la campana).

- **Muri**: ogni muro sulla linea retta tra chi fa rumore e chi ascolta lascia passare il 45% del raggio (fino a tre muri).
- **A schermo**: ogni rumore disegna a terra un cerchio chiaro largo quanto il suo raggio. È così che il giocatore "vede" il suono nella vista isometrica.
- Gli attrezzi fanno rumore: lavorare la serratura emette un rumore di 2,2 m ogni 0,6 secondi.

Limite noto: il suono viaggia in linea retta e non gira gli angoli. Una porta aperta oggi non lo lascia passare meglio di un muro.

### La guardia (`Watchman.cs`)

- **Vista**: cono di 76°, 14 metri. Il sospetto cresce con la visibilità del giocatore e con la vicinanza. Sotto 0,08 di visibilità non vede nulla, a meno di andarci a sbattere.
- **Udito**: sente i rumori il cui raggio, dopo i muri, lo raggiunge. Va a controllare nel punto del rumore.
- **Stati**: ronda → indagine (a sospetto 0,4) → allarme (a sospetto 1).
- Il sospetto cala di 0,22 al secondo quando non vede nulla.
- La sua lanterna è una luce come le altre: passandoti accanto ti illumina.
- La rete di navigazione si calcola da sola all'avvio del livello, a partire dai muri.

### Il principio della furtività

> **Difficile, non impossibile, e nessuna stupidità degli avversari.**
> — committente, 21 novembre 2026

Regole che ne derivano, e che ogni modifica all'IA deve rispettare:

1. **Nessun punto cieco comodo.** Stare in piedi e illuminati a pochi metri da una guardia non è mai sicuro, nemmeno alle sue spalle. Le guardie si guardano attorno.
2. **Basta poco per essere colti.** Il busto in luce nel cono della guardia è un allarme istantaneo: il volto è riconosciuto, è game over. Una spalla, un braccio o una gamba in luce generano un forte sospetto ma non una certezza: la guardia ha un tempo di reazione reale (un ladro o un bagliore?), ma è in tensione perché sorveglia e nulla la distrae. Cambia la ronda e gira bruscamente la testa verso vicoli e nascondigli.
3. **Il tempo è un avversario.** Più il giocatore indugia, più la guardia diventa attenta: si volta più spesso, allunga la ronda, con forti sospetti entra nei vicoli.
4. **Ogni minaccia si legge.** Prima di voltarsi la guardia dà un preavviso; un sospetto si vede dal segno sopra la testa e dal suo comportamento. Il giocatore perde per un errore suo, mai per un dado truccato.
5. **La riuscita si misura.** Esiste sempre un percorso, eseguito bene e con la tempistica giusta, che porta a termine il livello; un giocatore automatico lo verifica a ogni modifica (CR-027).

**Stato attuale rispetto al principio** (prove del 21 novembre 2026): avvicinarsi camminando funziona come deve, perché la guardia sente i passi a circa 4,5 m e scopre il giocatore in mezzo secondo. Ma un personaggio **fermo** di fianco o dietro la guardia, anche in piedi e illuminato dalla lanterna, resta invisibile per sempre: oltre 1,3 m la guardia vede solo nel cono frontale di 76°, e chi è fermo non fa rumore. Le voci CR-022…CR-027 del backlog correggono questo scarto.

---

## 7. Schermata di gioco

- **L'occhio** (in basso): chiuso e scuro se sei nascosto, aperto e chiaro se sei in luce.
- **Barra del rumore**, con andatura e superficie sotto i piedi.
- **Obiettivo** in alto a sinistra, una riga.
- **Segno d'allerta** in alto al centro: `?` quando una guardia sospetta, `!` rosso all'allarme. Lo stesso segno compare sopra la guardia.
- **Suggerimenti** in corsivo, uno alla volta.
- **Prima persona**: i bordi dello schermo si scuriscono, come da dentro un cappuccio.

### Le due visuali

| Visuale | Quando | Note |
|---|---|---|
| Isometrica | Quasi sempre | Ortografica, ruota a scatti di 90° |
| Prima persona, a comando | Il giocatore si ferma e guarda con i propri occhi | Le gambe sono bloccate: serve a osservare, non a muoversi |
| Prima persona, d'azione | Serrature, e in futuro buchi di serratura, uccisioni, borseggi | Entra ed esce da sola |

### Comandi

| Azione | Tasto |
|---|---|
| Muoversi | WASD o frecce |
| Furtivo | Ctrl (o C) |
| Correre | Shift |
| Interagire | E |
| Ruotare la visuale | Q, R |
| Guardare in prima persona | V |
| Pausa | Esc |
| Saltare il video | Spazio, Invio, Esc |

Si cambiano da Progetto > Impostazioni > Mappa input.

---

## 8. Struttura del progetto

```
thepalestitch_project/
  thepalestitch_prologue.tscn    livello del prologo
  scenes/      boot, intro, menu, character, player, ai, ui
  scripts/
    core/      GameState, SceneRouter, InputActions
    character/ CharacterCatalog (mestieri, vesti, volti), CharacterProfile (numeri derivati)
    stealth/   LightSensor, NoiseBus, NoiseRingSpawner
    player/    PlayerController, CameraRig, Mannequin
    ai/        Watchman
    world/     PrologueLevel, SurfaceZone, Interactable, TorchFlicker
    intro/     Boot, IntroCinematic, IntroShot
    ui/        MainMenu, CharacterCreation, Hud, PauseMenu, LightGem, PipBar, PortraitCard, StitchLine
  ui/          theme, fonts, shaders
  localization/strings.csv       tutti i testi, colonne en e it
```

Livelli fisici: 1 mondo, 2 giocatore, 3 guardie, 4 oggetti interattivi, 5 superfici.

Dove si cambia cosa:

| Voglio cambiare | File |
|---|---|
| Mestieri, vesti, tinte, volti, soprannomi | `scripts/character/CharacterCatalog.cs` |
| Quanto pesano abilità e vesti | `scripts/character/CharacterProfile.cs` |
| Qualsiasi testo | `localization/strings.csv` |
| Rumorosità delle superfici | `scripts/world/SurfaceZone.cs` |
| Velocità e rumore delle andature | proprietà del nodo `Player` |
| Sensi della guardia | proprietà del nodo `Watchman` |
| Inquadrature del video | marcatori sotto `Shots` in `IntroCinematic.tscn` |

---

## 9. Decisioni aperte e prossimi passi

Da decidere:

1. ~~**La premessa**~~ — confermata il 21 novembre 2026.
2. **Le doti dei mestieri** bastano come differenza, o si vuole anche un albero di crescita durante il gioco?
3. **Il volto** deve restare solo estetico, o cambiare le reazioni dei personaggi (un marchiato riconosciuto dalle guardie)?
4. **Edifici che coprono il personaggio** nella vista isometrica: oggi si ruota la camera. In alternativa, muri che diventano trasparenti o tetti che si scoperchiano.

Prossimi passi consigliati, in ordine:

1. Interno della casa del mercante (momenti D, E, F) con la scena del cadavere in prima persona.
2. Serratura come vero minigioco in prima persona.
3. Uccisione silenziosa e trasporto del corpo (abilità Lama e Nerbo).
4. Suono che passa da porte e finestre aperte.
5. Audio: passi per superficie, campana, voce narrante.
6. Salvataggio, per accendere "Riprendi".
7. Sostituzione dei blocchi con i modelli veri, a partire dal protagonista.
