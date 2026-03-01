
[Lien de génération](https://gemini.google.com/share/f0492459cbcc)
# 📝 Checklist TP1 : Poisson Dangereux

## 🛠️ Configuration & GitHub
- [ ] Dépôt créé via le lien Classroom et renommé `h26-tp1-DA` (ex: h26-tp1-1234567).
- [x] Version Unity **6000.2.13f1** utilisée.
- [ ] Fichier `.gitignore` (version Unity) présent à la racine.
- [x] Projet configuré en **3D**.

## 🌊 Environnement & Terrain
- [x] Plancher texturé avec l'image fournie.
- [x] 4 murs + 1 plafond transparents (Limites : X: -80 à 80, Y: -30 à 30, Z: -80 à 80).
- [x] Caméra : Background mis en **Solid Color** bleu (pas de Skybox par défaut).
- [x] Script `ModeTurbo.cs` attaché au Gestionnaire de Jeu.

## 🐠 Poisson-Clown (Joueur)
- [x] **Hiérarchie :** Parent vide à (0,0,0) avec Rigidbody + **BoxCollider** englobant.
- [x] **Apparence :** Sphères écrasées, texture orange et texture yeux appliquées.
- [x] **Déplacement (AddRelativeForce/Torque) :**
    - [x] Horizontal (A/D) : Rotation gauche/droite.
    - [x] Vertical (W/S) : Déplacement haut/bas.
    - [x] Espace : Avance vers l'avant (Z).
- [x] **Caméra :** Enfant du poisson, positionnée derrière.
- [x] **Queue :** Oscille entre -20° et +20°. Vitesse **2.5x** plus rapide quand Espace est enfoncé.
- [x] **Physique :** Damping (Drag) ajusté pour un contrôle agréable.

## 🪱 Appâts & Poissons Ennemis
- [x] **Appât :** Capsule (ver) + Sphère transparente (bulle).
- [x] **Ennemi :** Copie du joueur (plus gros, bleu/autre couleur).
- [x] **Spawn initial :** 3 appâts au hasard + 3 ennemis à 10 unités de leur appât (même Y).
- [x] **Animation Spawn :** Grossit graduellement de (0,0,0) à sa taille normale.
- [x] **Physique Ennemi :** `IsKinematic = true` par défaut. Devient `false` si touché.
- [x] **Rotation :** Animation aléatoire sur Y toutes les 0.5 à 2 secondes (quand Kinematic).

## 🎮 Logique de Gameplay
- [x] **Dérive :** L'ennemi dérive pendant **2.5s** après l'impact.
- [x] **Calcul Points :** $N^2$ points (N = nombre d'appâts touchés en une dérive).
- [x] **Nettoyage :** L'appât est détruit dès qu'un ennemi le touche.
- [x] **Reset :** Si aucun point marqué après 2.5s, l'ennemi retourne à sa position/rotation de départ.
- [x] **Progression :** Nouveau duo (Appât + Ennemi) toutes les $T$ secondes.
    - [x] Formule : $T = 5 - \text{score}^{0.25}$ (Minimum 2 secondes).

## 🫧 Bulles
- [x] **Apparition :** Groupes de 40 toutes les 0.5s à un endroit aléatoire au sol.
- [x] **Mouvement :** Montent verticalement.
- [x] **Collision :** Détruites dès qu'elles touchent un objet ou un mur.

## 🖥️ Interface Utilisateur (UI)
- [x] **HUD :** Score et Temps total affichés en haut à gauche.
- [x] **Feedback Score :** Texte vert avec Fade-in et Fade-out (Alpha TMP_Text).
- [x] **Animations Combo :** Si N > 1 (Jaune, durée 1s) :
    - [x] Choix aléatoire entre Clignotement ou "Shake" (pixel offset aléatoire).
    - [x] Protection anti-doublon (ne pas relancer si déjà en cours).
- [ ] **Fin de partie :** Déclenchée à 20 poissons.
    - [x] Affichage "PARTIE FINIE" + Score final.
    - [x] Relance du jeu après 5 secondes.

## ⌨️ Codes de Triche (Debug)
- [x] `u` : +1 point.
- [x] `i` : +4 points (déclenche Combo).
- [x] `o` : Lance animation Combo Clignote.
- [x] `l` : Lance animation Combo Shake.
- [x] `0` : Nettoie tous les ennemis et appâts.
- [x] `1` : Setup test (Poisson à 0,0,0 + 1 appât + 1 ennemi devant).
- [x] `2` : Setup combo (Poisson à 0,0,0 + 2 appâts + 1 ennemi devant).

## 📐 Architecture & Qualité
- [x] **Patron Observateur :** Utilisé pour lier la fin de la dérive au Gestionnaire de Jeu.
- [ ] **Code :** Clair, commenté, pas de duplication, pas de franglais.
- [ ] **Organisation :** Prefabs utilisés, dossiers Textures/Scripts/Prefabs propres.