---------
Food Pong
---------

Das Projekt wurde mit Vuforia 9.8.5 und einer Webcam entwickelt. Das Image Target wurde mit Affinity Designer 1.9 erstellt.


Skripte
-------
* AIPlayer.cs 		- veranwortlich für das Kontrollieren des AI Spielers
* FoodPongManger.cs	- veranwortlich für das Verwalten des Food Pong Spiels
* GameBall.cs		- kontrolliert den Spielball
* GameState.cs		- enthält ein Enum, welches den Spielzustand repräsentiert
* UIManager.cs		- verwaltet die UI für das Food Pong Spiel


Spielbeschreibung
-----------------
Der Spieler kann mit einem AI Gegner Pong spielen. Jeder Spieler hat drei Leben. Schafft es ein Spieler nicht, den Ball mit
seinem Schläger abzulenken verliert er ein Leben. Hat ein Spieler kein Leben mehr, ist das Spiel vorbei. Zusätzlich zum
Schläger kann der Spieler ein Objekt in das Spielfeld werfen. Trifft der Spielball das Objekt, bewegt sich der Spielball in
die entegengesetzte Richtung.


Vuforia Funktionalitäten
------------------------
* Ground Plane
	Alle Spielobjekte sind an ein Ground Plane angeheftet. Das Platzieren des Ground Planes geschieht über die Funktion
	'PerformHitTest' der Plane-Finder-Behavior-Komponente, die über die UI aufgerufen wird.

* Image Target
	Der Schläger des Spielers muss über ein Image Target in das Spiel gebracht werden. Indem der Spieler das Bild bewegt
	kann er den Schläger bewegen.



Bewegung des Spielballs
-----------------------
Der Spielball wird manuell über das Spielfeld bewegt (Transform.position) und nicht von dem Unity Physik-Framework
(Rigidbody.AddForce - das werfbare Objekt wurde mit dieser Funktion realisiert). Die Kollisionserkennung geschieht mit
Triggern (Collider.OnTriggerEnter). Collider haben die Tags "Bounce" (Ball wird von Wand oder Spielerschläger reflektiert),
"EnemyGoal" bzw. "PlayerGoal" (Ein Spieler verliert Leben) oder "ThrowableObstacle" (Ball trifft das Wurfobjekt und
bewegt sich in die entegengesetze Richtung).

Wird der Spielball reflektiert, geschieht die Berechnung der neuen Richtung über einen Raycast - ausgelöst von dem Trigger
Event. Der Raycast - ausgehend von dem Zentrum des Spielballs - verläuft in die Richtung in die sich der Spielball bewegt.
Das Ergebnis dieses Raycasts ist eine ungefähre Position, mit der der Spielball kolidieren würde. Mit dem Resultat und 
Vector3.Reflect wird die neue Richtung berechnet, in die sich der Spielball als nächstes bewegen soll. Mit der neuen
Richtung und einem weiteren Raycast (ausgehend von der Position des Treffers des letzten Raycasts) wird dann die neue
Target Position berechnet. Die verwendeten Raycasts berücksichtigen dabei nur Objekte, die dem selbstdefinierten
Unity Layer "Bounce" zugeordnet sind.

Offenes Problem:
Es kann vorkommen, dass das Zentrum des Spielballs die Targetposition erreicht, jedoch davor kein Trigger-Event ausgelöst
wurde. In diesem Fall wird als neues Target die Position eines Rettungspunktes gesetzt, der sich im gegnerischen Gebiet
befindet.
			


Externe Resourcen
-----------------
* Food and Kitchen Props Pack (für das Spielfeld, zuletzt aufgerufen am 01.06.21)
	https://assetstore.unity.com/packages/3d/props/food-and-kitchen-props-pack-85050

* CoffeeShop Starter Pack (für das Spielfeld, zuletzt aufgerufen am 01.06.21)
	https://assetstore.unity.com/packages/3d/props/coffeeshop-starter-pack-160914

* FREE Casual Food Pack- Mobile/VR (für das Spielfeld, zuletzt aufgerufen am 01.06.21)
	https://assetstore.unity.com/packages/3d/props/food/free-casual-food-pack-mobile-vr-85884

* Nudelholz-Karikatur (für das Image Target, zuletzt aufgerufen am 01.06.21)
	https://de.depositphotos.com/vector-images/teigrolle.html?qview=53507789

* Nahtlose Textur mit Küchenutensilien (für das Image Target, zuletzt aufgerufen am 01.06.21)
	https://de.depositphotos.com/vector-images/kitchen-doodle.html?qview=75508203