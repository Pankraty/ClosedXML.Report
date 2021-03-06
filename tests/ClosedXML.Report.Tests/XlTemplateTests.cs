﻿using ClosedXML.Excel;
using ClosedXML.Report.Tests.TestModels;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace ClosedXML.Report.Tests
{
    public class XlTemplateTests : XlsxTemplateTestsBase
    {
        public XlTemplateTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void Add_simple_variable_should_replace_value_in_related_cell()
        {
            XlTemplateTest("1.xlsx",
                tpl => tpl.AddVariable(new { TestValue1 = "value from test", TestValue2 = 3.2 }),
                wb =>
                {
                    var sheet = wb.Worksheet(1);
                    sheet.Cell(1, 1).HasFormula.Should().BeFalse();
                    sheet.Cell(1, 1).GetValue<string>().Should().Be("value from test");
                    sheet.Cell(2, 2).FormulaA1.Should().Be("7.2*2");
                    sheet.Cell(2, 2).GetValue<double>().Should().Be(14.4);
                });
        }

        [Fact(Skip = "https://github.com/ClosedXML/ClosedXML/issues/137")]
        public void Variables_test()
        {
            XlTemplateTest("7_vars.xlsx",
                tpl => {},
                wb =>
                {
                    var sheet = wb.Worksheet(1);
                    sheet.Cell("B5").GetValue<string>().Should().Be("10");
                });
        }

        [Fact]
        public void Add_nullable_variable_should_replace_value_in_related_cell()
        {
            XlTemplateTest("1.xlsx",
                tpl => tpl.AddVariable(new {TestValue2 = (double?) 2.3}),
                wb => wb.Worksheet(1).Cell(2, 2).Value.Should().Be((2.3 + 4) * 2)
            );
        }

        [Fact]
        public void Add_enumerable_of_simple_values_should_add_values_left_to_right()
        {
            XlTemplateTest("3.xlsx",
                tpl => tpl.AddVariable(new
                {
                    title = "title from test",
                    TestArray = new[] { 10, 22, 8, 4 }
                }),
                wb =>
                {
                    var sheet = wb.Worksheet(1);
                    sheet.Cell(1, 4).GetValue<string>().Should().Be("title from test");
                    sheet.Cell(2, 3).GetValue<int>().Should().Be(10);
                    sheet.Cell(2, 4).GetValue<int>().Should().Be(22);
                    sheet.Cell(2, 5).GetValue<int>().Should().Be(8);
                    sheet.Cell(2, 6).GetValue<int>().Should().Be(4);
                    sheet.Cell(2, 7).GetValue<int>().Should().Be(44);
                });
        }

        [Fact]
        public void Add_enumerable_variable_should_fill_range()
        {
            XlTemplateTest("4.xlsx",
                tpl => tpl.AddVariable(new
                {
                    title = "title from test",
                    dates = new[] { DateTime.Parse("2013-01-01"), DateTime.Parse("2013-01-02"), DateTime.Parse("2013-01-03") },
                    PlanData = TestEntity.GetTestData(3)
                }),
                wb =>
                {
                    var sheet = wb.Worksheet(1);
                    sheet.Cell("F1").GetValue<string>().Should().Be("title from test");
                    sheet.Cell("B4").GetValue<string>().Should().Be("John Smith");
                    sheet.Cell("B5").GetValue<string>().Should().Be("James Smith");
                    sheet.Cell("B6").GetValue<string>().Should().Be("Jim Smith");
                    sheet.Cell("C4").GetValue<string>().Should().Be("Developer");
                    sheet.Cell("C5").GetValue<string>().Should().Be("Analyst");
                    sheet.Cell("C6").GetValue<string>().Should().Be("Manager");
                    sheet.Cell("D4").GetValue<int>().Should().Be(24);
                    sheet.Cell("D5").GetValue<int>().Should().Be(37);
                    sheet.Cell("D6").GetValue<int>().Should().Be(31);
                    sheet.Cell("E4").GetValue<string>().Should().Be("NY");
                    sheet.Cell("E5").GetValue<string>().Should().Be("Dallas");
                    sheet.Cell("E6").GetValue<string>().Should().Be("Miami");
                    wb.NamedRange("PlanData").Ranges.First().RangeAddress.ToStringRelative().Should().Be("A4:I6");
                    sheet.Cell("F4").GetValue<int>().Should().Be(6);
                    sheet.Cell("F5").GetValue<int>().Should().Be(3);
                    sheet.Cell("F6").GetValue<int>().Should().Be(2);
                    sheet.Cell("G4").GetValue<int>().Should().Be(8);
                    sheet.Cell("G5").GetValue<int>().Should().Be(5);
                    sheet.Cell("G6").GetValue<int>().Should().Be(9);
                    sheet.Cell("H4").GetValue<int>().Should().Be(4);
                    sheet.Cell("H5").GetValue<int>().Should().Be(7);
                    sheet.Cell("H6").GetValue<int>().Should().Be(1);
                    sheet.Cell("I4").GetValue<int>().Should().Be(18);
                    sheet.Cell("I5").GetValue<int>().Should().Be(15);
                    sheet.Cell("I6").GetValue<int>().Should().Be(12);
                    sheet.Cell("D8").GetValue<int>().Should().Be(15);
                    sheet.Cell("J6").GetValue<int>().Should().Be(4);
                });
        }

        [Fact]
        public void Add_complex_object_shold_replace_all_possible_values()
        {
            XlTemplateTest("2.xlsx",
                tpl => tpl.AddVariable(new
                {
                    title = "title from test",
                    birthdate = new DateTime(2009, 8, 17, 16, 40, 33),
                    dates = new[] { DateTime.Parse("2013-01-01"), DateTime.Parse("2013-01-02"), DateTime.Parse("2013-01-03") },
                    person = new
                    {
                        age = 35,
                        name = "Пупкин Иван",
                        car = new
                        {
                            brand = "Mercedes-Benz",
                            model = "C230"
                        }
                    },
                }),
                wb =>
                {
                    var sheet = wb.Worksheet(1);
                    sheet.Cell("B2").GetValue<string>().Should().Be("title from test");
                    sheet.Cell("C4").GetValue<int>().Should().Be(35);
                    sheet.Cell("C5").GetValue<DateTime>().Should().Be(new DateTime(2009, 8, 17, 16, 40, 33));
                    sheet.Cell("C6").GetValue<string>().Should().Be("Пупкин Иван");
                    sheet.Cell("C8").GetValue<string>().Should().Be("Mercedes-Benz");
                    sheet.Cell("C9").GetValue<string>().Should().Be("C230");
                    sheet.Cell("D11").GetValue<DateTime>().Should().Be(DateTime.Parse("2013-01-01"));
                    sheet.Cell("E11").GetValue<DateTime>().Should().Be(DateTime.Parse("2013-01-02"));
                    sheet.Cell("F11").GetValue<DateTime>().Should().Be(DateTime.Parse("2013-01-03"));
                    sheet.Cell("H11").GetValue<string>().Should().Be("should stay");
                }
            );
        }

        [Fact]
        public void RowWideRangesProcessedCorrectly()
        {
            var workbook = CreateWorkbook();
            var sheet = workbook.Worksheets.First();
            var items = GenerateItems();

            sheet.Range("2:3").AddToNamed("Items");

            var template = new XLTemplate(workbook);
            template.AddVariable("Items", items);
            template.Generate();

            sheet.Cell("B2").Value.Should().Be("Alice");
            sheet.Cell("B3").Value.Should().Be("Bob");
            sheet.Cell("B4").Value.Should().Be("Carl");

            sheet.Cell("C2").Value.Should().Be(20.0);
            sheet.Cell("C3").Value.Should().Be(30.0);
            sheet.Cell("C4").Value.Should().Be(38.0);

            sheet.Cell("F2").Value.Should().Be("Placeholder");
            sheet.Cell("F3").Value.Should().Be("Placeholder");
            sheet.Cell("F4").Value.Should().Be("Placeholder");

            XLWorkbook CreateWorkbook()
            {
                var wb = new XLWorkbook();
                var ws = wb.AddWorksheet("Sheet1");
                ws.Cell("B1").Value = "Name";
                ws.Cell("C1").Value = "Age";
                ws.Cell("B2").Value = "{{item.Name}}";
                ws.Cell("C2").Value = "{{item.Age}}";

                ws.Cell("F2").Value = "Placeholder";
                return wb;
            }

            IEnumerable<dynamic> GenerateItems()
            {
                return new List<dynamic>
                {
                    new { Name = "Alice", Age = 20},
                    new { Name = "Bob", Age = 30},
                    new { Name = "Carl", Age = 38},
                };
            }
        }
    }
}
