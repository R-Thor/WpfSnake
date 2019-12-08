namespace WpfSnake.NetCore.Common
{
    internal class Location
    {
        internal int Row { get; set; }
        internal int Column { get; set; }
        internal Location()
        {

        }
        internal Location(int row, int col)
        {
            this.Row = row;
            this.Column = col;
        }
    }
}
