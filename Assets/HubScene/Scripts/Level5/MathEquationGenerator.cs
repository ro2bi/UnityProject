public static class MathEquationGenerator
{
    public static string GetEquation(int result)
    {
        return result switch
        {
            1 => "5 - 4",
            2 => "√16 - 2",
            3 => "9 / 3",
            4 => "2 + 2",
            5 => "25 / 5",
            6 => "3 * 2",
            7 => "14 / 2",
            8 => "2^3",
            _ => "0"
        };
    }
}