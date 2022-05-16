public static class Extension
{
    public enum BodyPartEnum {armRight,armLeft,center,footRight,footLeft}
    public enum DataReqEnum {rotX,rotY,accX,accY,accZ}
    public enum AnimationControlEnum { rotation,position,scale,acceleration,color}
    public enum DirectionEnum { x,y,z}
    public static float Remap(this float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

    
}
