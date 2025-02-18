using System;

[Flags]
public enum EDetectableObjectCategories {
    None = 0,
    // Rabbit Related Objects
    RABBIT = 1,
    RABBIT_FOOD = 2,

    //Fox Related Objects
    FOX = 4,

    // Other types of detectable objects
    TALL_GRASS = 8
}
