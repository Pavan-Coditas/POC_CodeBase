using EmployeeManagement.Domain.Models.RulesEngineModels;
using EmployeeManagement.Entities.Models.EntityModels;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;

namespace RuleEnginePerformanceTest
{

    public class RuleEngine
    {
        private readonly List<WorkflowRule> _workflowRules;

        public RuleEngine(List<WorkflowRule> workflowRules)
        {
            _workflowRules = workflowRules;
        }

        public List<Person> CalculateSalary(List<Person> employees)
        {
            var salaryCalculationWorkflow = _workflowRules.Find(w => w.WorkflowName == "SalaryCalculations");

            var employeeResult = new List<Person>();

            foreach (var employee in employees)
            {
                foreach (var rule in salaryCalculationWorkflow!.Rules!)
                {
                    var parameterExpression = Expression.Parameter(typeof(RuleParameter), "RuleParameter");
                    var ruleExpression = DynamicExpressionParser.ParseLambda(new[] { parameterExpression }, typeof(bool), rule.Expression!);
                    var ruleLambda = ruleExpression.Compile();
                    var isRuleSatisfied = (bool)ruleLambda.DynamicInvoke(new RuleParameter { ExperienceInYears = employee.Experience })!;

                    if (isRuleSatisfied)
                    {
                        employee.Salary = rule.Salary;
                        break;
                    }


                }

                employeeResult.Add(employee);
            }
            return employeeResult;
        }
    }
    public class RuleParameter
    {
        public int? ExperienceInYears { get; set; }

    }

}
