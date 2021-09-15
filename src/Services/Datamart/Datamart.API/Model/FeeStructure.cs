using System;

namespace AHAM.Services.Datamart.API.Model
{
    public class FeeStructure : BaseModel
    {
        public DateTime EffectiveFrom { get; set; }
        public Product Product { get; set; }
        public Account Account { get; set; }
        public Agent Agent { get; set; }
        public string Plan { get; set; }
        public int FeeType { get; set; }
        public int Basis { get; set; }
        public double AmountFrom { get; set; }
        public double AmountTo { get; set; }
        public double Rate { get; set; }
    }
}