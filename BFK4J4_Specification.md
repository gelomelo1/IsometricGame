Unity 3D Wave-alapú Zombie Shooter – Specifikáció
=======================================================

Koncepció és motiváció
-------------------------

A projekt célja egy izometrikus nézetű, 3D-s akciójáték fejlesztése Unity motorral, ahol a játékos egy katonát irányít, és folyamatosan érkező zombihullámokat kell túléljen. A játék fő motivációja a gyors reakciók, taktikai mozgás és erőforrás-kezelés kombinálása. A wave-alapú rendszer egyre növekvő nehézséget biztosít, ami folyamatos kihívást jelent a játékos számára. A játék támogatja mind a véges (fix waveszám), mind a végtelen (endless) játékmódot. A cél egy élvezetes, újrajátszható élmény létrehozása.

* * *

Use-Case diagram
--------------------

![use case diagram](file:///E:/Homework/Programming/Unity/Isometric%20Game/usecase_diagram.png)

<!-- IDE KÉSZÍTSD MAJD A DIAGRAMOT -->

* * *

Use-Case-ek
--------------

* Játék indítása
* Játékmód kiválasztása (fix wave / endless)
* Waveszám beállítása
* Karakter mozgatása (futás, sprint, ugrás, gurulás)
* Fegyverhasználat (pisztoly, gépfegyver, gránát)
* Fegyver újratöltése
* Ellenfelek (zombik) leküzdése
* Lőszer felvétele
* Életerő regeneráció
* Wave teljesítése
* Játék vége (győzelem/vereség)

* * *

User Story-k
---------------

### Alap játékélmény

**Mint játékos**, szeretnék zombikat lelőni különböző fegyverekkel,  
**hogy** túléljem az egyre nehezebb hullámokat.

* * *

### Játékmód választás

**Mint játékos**, szeretném kiválasztani, hogy hány wave-et akarok játszani vagy endless módot,  
**hogy** a saját tempómhoz és időmhöz igazíthassam a játékot.

* * *

### Mozgás és stamina

**Mint játékos**, szeretnék különböző mozgásformákat használni (sprint, ugrás, gurulás),  
**hogy** hatékonyabban tudjam elkerülni az ellenfeleket, de figyelnem kelljen a stamina szintre.

* * *

### Fegyverhasználat

**Mint játékos**, szeretnék többféle fegyvert használni (pisztoly, gépfegyver, gránát),  
**hogy** különböző stratégiákat alkalmazhassak a zombik ellen.

* * *

### Erőforrás menedzsment

**Mint játékos**, szeretném, hogy a lőszer korlátozott legyen és újra kelljen tölteni,  
**hogy** taktikusabban kelljen harcolnom.

* * *

### Regeneráció

**Mint játékos**, szeretném, hogy az életem regenerálódjon, ha nem sebződöm egy ideig,  
**hogy** legyen esélyem visszatérni nehéz helyzetekből.

* * *

### Nehézség növekedés

**Mint játékos**, szeretném, hogy minden wave nehezebb legyen az előzőnél,  
**hogy** folyamatos kihívást jelentsen a játék.

* * *

Alkalmazás összefoglaló
----------------------------------------

Az alkalmazás egy Unity-ben fejlesztett izometrikus nézetű 3D akciójáték, amelyben a játékos egy katonát irányít zombihullámok ellen. A játékmenet wave-alapú, ahol az ellenfelek hullámokban érkeznek, és minden újabb hullám egyre több és erősebb zombit tartalmaz. A játékos célja az összes hullám túlélése, vagy endless módban minél tovább életben maradni.

A játék két fő módot kínál: fix waveszámú módot, ahol a játékos előre beállíthatja a hullámok számát, valamint végtelen módot, ahol a játék addig tart, amíg a játékos életben marad. Egy wave akkor ér véget, ha a játékos minden ellenfelet legyőzött, és az új hullám 5 másodperccel később indul.

A játékos különböző mozgási lehetőségekkel rendelkezik: futás, sprintelés, ugrás és gurulás, amelyek stamina használatához kötöttek. A stamina menedzsment kulcsfontosságú a túléléshez. Harc közben a játékos három különböző fegyvert használhat: pisztolyt, gépfegyvert és gránátot, amelyek mindegyike újratöltést igényel.

A játék tartalmaz erőforrás-menedzsment elemeket is: a lőszer limitált, de az ellenfelek legyőzése után véletlenszerűen spawnolhat. A játékos életereje automatikusan regenerálódik, ha egy ideig nem éri sebzés, ami taktikai visszavonulást tesz lehetővé.

A játék véget ér, ha a játékos élete nullára csökken, vagy ha sikeresen teljesíti az összes előre beállított wave-et. A cél egy dinamikus, kihívásokkal teli és újrajátszható játékélmény biztosítása.
