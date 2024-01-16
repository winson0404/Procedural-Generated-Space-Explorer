namespace TwoManSky
{
    class Planet
    {
        public PlanetType planetType;
        public uint planetSize;
        public uint planetDifficulty;

        public Planet(uint planetTypeInt, uint planetSize, uint planetDifficulty)
        {
            this.planetType = (PlanetType)planetTypeInt;
            this.planetSize = planetSize;
            this.planetDifficulty = planetDifficulty;
        }
    }

    enum PlanetType
    {
        GasGiant,
        IceGiant,
        Desert,
        Barren,
        Volcanic,
        Terran
    }
}