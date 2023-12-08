namespace DBAM.Serialization;

public static class BinaryDeserializer
{
    public static AnimationData Deserialize(FileStream dbamFileStream)
    {
        AnimationData animation = new();
        
        using (BinaryReader binaryReader = new BinaryReader(dbamFileStream))
        {
            ArrangementData[] arrangements = new ArrangementData[binaryReader.ReadUInt16()];
            for (int i = 0; i < arrangements.Length; i++)
            {
                ArrangementData arrangement = new();
                SpriteData[] sprites = new SpriteData[binaryReader.ReadUInt16()];

                for (int j = 0; j < sprites.Length; j++)
                {
                    SpriteData sprite = new SpriteData
                    {
                        // Unlike every other value, the sprite part's world position is signed.
                        WorldPositionX = binaryReader.ReadInt16(),
                        WorldPositionY = binaryReader.ReadInt16(),
                        
                        TextureSizeX = binaryReader.ReadUInt16(),
                        TextureSizeY = binaryReader.ReadUInt16(),
                        
                        TexturePositionX = binaryReader.ReadUInt16(),
                        TexturePositionY = binaryReader.ReadUInt16(),
                    };

                    sprites[j] = sprite;
                    
                    // Skip the next 4 bytes, there's some padding between sprites for some reason.
                    binaryReader.ReadUInt32();
                }

                arrangement.Sprites = sprites;
                arrangements[i] = arrangement;
            }

            animation.Arrangements = arrangements;

            // Some flags that come here.
            // I don't think they're particularly useful. I managed to change the visibility with them, perhaps something like the unit's shadow graphic is also set using this?
            ushort _uselessFlags = binaryReader.ReadUInt16();

            FrameData[] frames = new FrameData[binaryReader.ReadUInt16()];

            for (int i = 0; i < frames.Length; i++)
            {
                FrameData frame = new FrameData
                {
                    ArrangementIndex = binaryReader.ReadUInt16(),
                    StopLength = binaryReader.ReadUInt16(),
                };

                frames[i] = frame;
                
                // Seek to the next frame. Not sure what goes here.
                for (int j = 0; j < 8; j++)
                {
                    binaryReader.ReadUInt16();
                }
            }
            
            animation.Frames = frames;
        }
        
        return animation;
    }
    

    public static AnimationData Deserialize(string dbamPath)
    {
        using FileStream fileStream = new FileStream(dbamPath, FileMode.Open);
        return Deserialize(fileStream);
    }
}
