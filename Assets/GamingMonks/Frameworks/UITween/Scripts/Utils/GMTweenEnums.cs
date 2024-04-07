using UnityEngine;

namespace GamingMonks.UITween 
{
    // Ease type.
    public enum Ease
    {
        Linear,
        EaseIn,
        EaseOut,
        Custom
	}

    // Loop Type of Tween.
    public enum LoopType {
        PingPong,
        Loop
    }

    // Tween Type.
    public enum AnimationType {
        AnchorX,
        AnchorY,
        AnchorZ,
        AnchoredPosition,
        AnchoredPosition3D,

        PositionX,
        PositionY,
        PositionZ,
        Position,

        LocalPositionX,
        LocalPositionY,
        LocalPositionZ,
        LocalPosition,

        LocalRotationTo,
        LocalRotationToX,
        LocalRotationToY,
        LocalRotationToZ,

        LocalScaleX,
        LocalScaleY,
        LocalScaleZ,
        LocalScale,

        RotateByX,
        RotateByY,
        RotateByZ,

        RotateToX,
        RotateToY,
        RotateToZ,

        RotateBy,
        RotateTo,

        Alpha,
        Color,
        FillAmount
    }

    public enum ImageType {
        Image,
        SpriteRenderer
    }
}
