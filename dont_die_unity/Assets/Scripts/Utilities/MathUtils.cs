public static class MathUtils
{
    // Extension method for float array
    public static float Average(this float [] array)
    {
        float average = 0;
        int count = array.Length;
        for (int i = 0; i < count; i++)
        {
            average += array[i];
        }
        return average / count;
    }
}