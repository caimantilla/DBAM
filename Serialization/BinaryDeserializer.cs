using System;
using System.IO;

namespace DBAM.Serialization
{
    public static class BinaryDeserializer
    {
        public static AnimationData Deserialize(FileStream dbamFileStream)
        {
            var animation = new AnimationData();

            using (BinaryReader binaryReader = new BinaryReader(dbamFileStream))
            {
                ArrangementData[] arrangements = new ArrangementData[binaryReader.ReadUInt16()];
                for (int i = 0; i < arrangements.Length; i++)
                {
                    var arrangement = new ArrangementData();
                    SpriteData[] sprites = new SpriteData[binaryReader.ReadUInt16()];

                    for (int j = 0; j < sprites.Length; j++)
                    {
                        // Each sprite is 16 bytes long, but what comes after the 12th byte is a bit of a mystery.
                        SpriteData sprite = new SpriteData
                        {
                            // Unlike every other value, the sprite part's world position is signed.
                            WorldPositionX = binaryReader.ReadInt16(),
                            WorldPositionY = binaryReader.ReadInt16(),

                            TextureSizeX = binaryReader.ReadUInt16(),
                            TextureSizeY = binaryReader.ReadUInt16(),

                            TexturePositionX = binaryReader.ReadUInt16(),
                            TexturePositionY = binaryReader.ReadUInt16(),

                            FlipH = binaryReader.ReadBoolean(),
                            _UnknownVal1 = binaryReader.ReadBoolean(),
                            _UnknownVal2 = binaryReader.ReadBoolean(),
                            _UnknownVal3 = binaryReader.ReadBoolean(),
                        };

                        sprites[j] = sprite;
                    }

                    arrangement.Sprites = sprites;
                    arrangements[i] = arrangement;
                }

                animation.Arrangements = arrangements;

                // Some flags that come here.
                // I don't think they're particularly useful. I managed to change the visibility with them, perhaps something like the unit's shadow graphic is also set using this?
                ushort _unkownFlags = binaryReader.ReadUInt16();

                FrameData[] frames = new FrameData[binaryReader.ReadUInt16()];

                for (int i = 0; i < frames.Length; i++)
                {
                    FrameData frame = new FrameData
                    {
                        ArrangementIndex = binaryReader.ReadUInt16(),
                        StopLength = binaryReader.ReadUInt16(),
                    };

                    frames[i] = frame;

                    // Seek to the next frame. Not sure what goes here. Maybe some kinds of effects triggers...?
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
}
