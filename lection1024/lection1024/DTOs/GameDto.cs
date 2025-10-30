namespace lection1024.DTOs
{
    public class GameDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;   
        public string Category { get; set; } = null!;
        public decimal Price { get; set; }
    }

    public static class GameExtenstions
    {
        public static GameDto ToDto(this GameDto g)
            => new()
    }
}
