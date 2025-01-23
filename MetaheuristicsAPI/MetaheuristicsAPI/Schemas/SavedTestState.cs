namespace MetaheuristicsAPI.Schemas
{
    public class SavedTestState {
        public int lastTestIndex;
        public TestRequest[] requests;
        public TestResults[] results;
        public bool testMultiple;

        public SavedTestState(int lastTestIndex, TestRequest[] requests, TestResults[] results, bool testMultiple)
        {
            this.lastTestIndex = lastTestIndex;
            this.requests = requests;
            this.results = results;
            this.testMultiple = testMultiple;
        }
    }
}
