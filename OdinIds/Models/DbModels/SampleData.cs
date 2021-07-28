namespace OdinIds.Models.DbModels
{
    public class SampleData
    {
        public SampleData()
        {

        }

        public static void Init(OdinIdentityEntities context)
        {
            context.Stus.Add(new Student_DbModel
            {
                Id = 1,
                StudentName = "odinsam"
            });
            context.SaveChanges();
        }
    }
}