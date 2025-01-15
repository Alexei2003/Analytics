namespace Analytics.Server.Objects
{
    public class Analytics
    {
        public DataChartStr GenderChart { get; set; } = new();
        public DataChartStr MaritalStatusChart { get; set; } = new();
        public DataChartStr AgeChart { get; set; } = new();
        public DataChartStr WorkExperienceChart { get; set; } = new();
        public DataChartStr WorkExperienceMeanChart { get; set; } = new();
        public float WorkExperienceMean { get; set; } = 0f;

        public class DataChartStr()
        {
            public Dictionary<string, int> Data { get; set; } = new();

            public void Inc(string key, int value = 1)
            {
                if (!Data.TryAdd(key, 1))
                {
                    Data[key] += value;
                }
            }

            public int Get(string key)
            {
                return Data[key];
            }

            public int Change(string key, int value)
            {
                return Data[key] = value;
            }
            public int Add(string key, int value)
            {
                return Data[key] = value;
            }

            public void Remove(string key)
            {
                Data.Remove(key);
            }
        }
    }
}
