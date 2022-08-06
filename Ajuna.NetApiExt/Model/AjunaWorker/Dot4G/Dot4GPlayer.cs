namespace Ajuna.NetApiExt.Model.AjunaWorker.Dot4G
{
    public class Dot4GPlayer
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public int Bombs { get; internal set; }
        public int Stone { get; internal set; }

        public Dot4GPlayer(string name, string address, int stone)
        {
            Name = name;
            Address = address;
            Stone = stone;
        }

        override
        public string ToString()
        {
            return $"{Name} - Bomb[{Bombs}]";  
        }
    }
}