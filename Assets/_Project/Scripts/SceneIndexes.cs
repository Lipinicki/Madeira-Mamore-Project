using System;

// Enum Representing the scenes playable in the game
// Each name/index should match it's actual scene names and indexes in scene manager
[Serializable]
public enum SceneIndexes
{
	TitleScreen = 1, //NOTE: Don't change this without changing all it's references
	Fase1 = 2, 
	Fase2 = 3,
	Fase3 = 4,
	Fase4 = 5,
	Credits = 6,
}
