//the standart unity vector3 data type isnt serializable,
//therefore we create a our own vectr3 class and make it a [System.Serializable]
[System.Serializable]
public class Vector3Serializable
{
    public float x, y, z;

    public Vector3Serializable(float x, float y, float z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public Vector3Serializable()
    {
    }
}
