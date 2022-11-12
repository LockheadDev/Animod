public static class Extension
{
    public enum BodyPartEnum {armRight,armLeft,center,legRight,legLeft}
    public enum DataReqEnum {rotX,rotY,accX,accY,accZ}
    public enum AnimationControlEnum { rotation,position,scale,acceleration,color, pitch, volume,pan,LPF}
    public enum DirectionEnum { none,x,y,z}
    public enum AudioChannelEnum { none, channel1,channel2, channel3, channel4, channel5, channel6, channel7 , channel8 }
    public enum ColorEnum { none, r,g,b}

    public static float Remap(this float value, float from1, float to1, float from2, float to2)
    {
        float temp = 0f;

        if (value < from1) 
        { 
            temp = from2; 
        }
        else if (value > to1) 
        { 
            temp = to2; 
        } 
        else
        {
            temp= (value - from1) / (to1 - from1) * (to2 - from2) + from2;
        }
        return temp;
    }

    
}
