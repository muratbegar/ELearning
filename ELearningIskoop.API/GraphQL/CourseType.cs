using ELearningIskoop.Courses.Application.Queries.GetCourse;

namespace ELearningIskoop.API.GraphQL
{
    public class CourseType : ObjectType<GetCourseResponse>
    {
        protected override void Configure(IObjectTypeDescriptor<GetCourseResponse> descriptor)
        {
            descriptor.Description("Represents a course in the e-learning platform");

            descriptor.Field(x => x.ObjectId).Description("The unique identifier of the course");

            descriptor
                .Field(c => c.Title)
                .Description("The title of the course");

            descriptor
                .Field(c => c.Description)
                .Description("The detailed description of the course");

            descriptor
                .Field(c => c.InstructorName)
                .Description("The full name of the course instructor");

            descriptor
                .Field(c => c.FormattedPrice)
                .Description("The formatted price of the course with currency");

            descriptor
                .Field(c => c.IsFree)
                .Description("Indicates whether the course is free");

            descriptor
                .Field(c => c.Level)
                .Description("The difficulty level of the course");

            descriptor
                .Field(c => c.Status)
                .Description("The current status of the course");

            descriptor
                .Field(c => c.TotalDuration)
                .Description("The total duration of the course");

            descriptor
                .Field(c => c.CurrentStudentCount)
                .Description("The current number of enrolled students");

            descriptor
                .Field(c => c.MaxStudents)
                .Description("The maximum number of students allowed");

            descriptor
                .Field(c => c.IsAvailable)
                .Description("Indicates whether the course is available for enrollment");

            descriptor
                .Field(c => c.Lessons)
                .Description("The lessons included in this course");

            descriptor
                .Field(c => c.Categories)
                .Description("The categories this course belongs to");
        }
    }
}
