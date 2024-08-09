public class MapData
{
    public double Lat { get; set; }
    public double Long { get; set; }
    public string LatestAdress { get; set; }

    public override string ToString()
    {
        return $"Lat: {Lat}, Long: {Long}, Address: {LatestAdress}";
    }
}
