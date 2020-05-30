namespace ETF.TripleTriad
{
    public class LastItemChosen
    {
        public readonly int pageNum;
        public readonly int spotOnPage;
        public readonly int spotInCardInv;



        public LastItemChosen(int pageNum, int spotOnPage, int spotInCardInv)
        {
            this.pageNum = pageNum;
            this.spotOnPage = spotOnPage;
            this.spotInCardInv = spotInCardInv;
        }

    }

}