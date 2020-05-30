using System.IO;
using UnityEngine;

public class GameDataReader 
{
    //Version doesn't change, no need for a setter
    public int Version { get; }
    BinaryReader reader;

    //Constructor, initialising the reader
    public GameDataReader(BinaryReader reader)
    {
        this.reader = reader;
    }

    public string ReadString()
    {
        string value = "";
        //reading the length of the string 
        int charCount = reader.ReadChar();

        for(int i = 0; i < charCount; i++)
        {
            value += reader.ReadChar();
        }
        return value;
    }

    public float ReadFloat()
    {
        return reader.ReadSingle();
    }
    public int ReadInt()
    {
        return reader.ReadInt32();
    }
    public Quaternion ReadQuaternion()
    {
        Quaternion value;
        value.x = reader.ReadSingle();
        value.y = reader.ReadSingle();
        value.z = reader.ReadSingle();
        value.w = reader.ReadSingle();
        return value;
    }
    public Vector3 ReadVector3()
    {
        Vector3 value;
        value.x = reader.ReadSingle();
        value.y = reader.ReadSingle();
        value.z = reader.ReadSingle();
        return value;
    }

    public Color ReadColor()
    {
        Color value;
        value.r = reader.ReadSingle();
        value.g = reader.ReadSingle();
        value.b = reader.ReadSingle();
        value.a = reader.ReadSingle();
        return value;
    }
}
