using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SistemaAcademico.Models;

namespace SistemaAcademico.Services;

public class PdfService
{
    public byte[] GenerarReporteNotas(ReporteNotas reporte)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.Letter);
                page.Margin(2, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(11));

                page.Header()
                    .AlignCenter()
                    .Column(column =>
                    {
                        column.Item().Text("SISTEMA DE GESTIÓN ACADÉMICA")
                            .FontSize(18).Bold().FontColor(Colors.Blue.Darken2);
                        column.Item().Text("Reporte de Calificaciones")
                            .FontSize(14).SemiBold();
                        column.Item().PaddingTop(5).LineHorizontal(1).LineColor(Colors.Grey.Lighten1);
                    });

                page.Content()
                    .PaddingVertical(1, Unit.Centimetre)
                    .Column(column =>
                    {
                        // Información del estudiante
                        column.Item().Row(row =>
                        {
                            row.RelativeItem().Column(col =>
                            {
                                col.Item().Text($"Estudiante: {reporte.NombreCompleto}").Bold();
                                col.Item().Text($"Documento: {reporte.Documento}");
                                col.Item().Text($"Grado: {reporte.Grado} - Sección {reporte.Seccion}");
                            });
                            row.RelativeItem().Column(col =>
                            {
                                col.Item().AlignRight().Text($"Ciclo Escolar: {reporte.CicloEscolar}").Bold();
                                col.Item().AlignRight().Text($"Fecha: {DateTime.Now:dd/MM/yyyy}");
                            });
                        });

                        column.Item().PaddingVertical(10);

                        // Tabla de calificaciones
                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(3);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(1);
                            });

                            // Encabezado
                            table.Header(header =>
                            {
                                header.Cell().Element(CellStyle).Background(Colors.Blue.Darken2).Text("Asignatura").FontColor(Colors.White).Bold();
                                header.Cell().Element(CellStyle).Background(Colors.Blue.Darken2).Text("Periodo").FontColor(Colors.White).Bold();
                                header.Cell().Element(CellStyle).Background(Colors.Blue.Darken2).Text("Nota").FontColor(Colors.White).Bold();
                                header.Cell().Element(CellStyle).Background(Colors.Blue.Darken2).Text("Estado").FontColor(Colors.White).Bold();
                            });

                            // Datos
                            foreach (var nota in reporte.Notas)
                            {
                                table.Cell().Element(CellStyle).Text(nota.Asignatura);
                                table.Cell().Element(CellStyle).Text(nota.Periodo);
                                table.Cell().Element(CellStyle).AlignCenter().Text(nota.Nota.ToString("F2"));
                                table.Cell().Element(CellStyle).AlignCenter()
                                    .Text(nota.Estado)
                                    .FontColor(nota.Estado == "Aprobado" ? Colors.Green.Darken2 : Colors.Red.Darken2)
                                    .Bold();
                            }
                        });

                        column.Item().PaddingTop(15);

                        // Promedio general
                        column.Item().AlignRight().Row(row =>
                        {
                            row.AutoItem().Background(Colors.Blue.Lighten3).Padding(10).Text($"PROMEDIO GENERAL: {reporte.PromedioGeneral:F2}")
                                .FontSize(14).Bold().FontColor(Colors.Blue.Darken3);
                        });
                    });

                page.Footer()
                    .AlignCenter()
                    .Text(x =>
                    {
                        x.Span("Página ");
                        x.CurrentPageNumber();
                        x.Span(" de ");
                        x.TotalPages();
                    });
            });
        });

        return document.GeneratePdf();
    }

    public byte[] GenerarListadoMatriculas(List<ReporteMatricula> matriculas, string cicloEscolar)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.Letter.Landscape());
                page.Margin(1.5f, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(10));

                page.Header()
                    .AlignCenter()
                    .Column(column =>
                    {
                        column.Item().Text("SISTEMA DE GESTIÓN ACADÉMICA")
                            .FontSize(16).Bold().FontColor(Colors.Blue.Darken2);
                        column.Item().Text($"Listado de Matrículas - Ciclo Escolar {cicloEscolar}")
                            .FontSize(12).SemiBold();
                        column.Item().Text($"Generado: {DateTime.Now:dd/MM/yyyy HH:mm}")
                            .FontSize(9).FontColor(Colors.Grey.Darken1);
                        column.Item().PaddingTop(5).LineHorizontal(1).LineColor(Colors.Grey.Lighten1);
                    });

                page.Content()
                    .PaddingVertical(0.5f, Unit.Centimetre)
                    .Column(column =>
                    {
                        column.Item().Text($"Total de estudiantes matriculados: {matriculas.Count}")
                            .FontSize(11).Bold().FontColor(Colors.Blue.Darken2);

                        column.Item().PaddingVertical(5);

                        // Tabla de matrículas
                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(30);
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(1.5f);
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(1);
                            });

                            // Encabezado
                            table.Header(header =>
                            {
                                header.Cell().Element(CellStyle).Background(Colors.Blue.Darken2).Text("N°").FontColor(Colors.White).Bold();
                                header.Cell().Element(CellStyle).Background(Colors.Blue.Darken2).Text("Documento").FontColor(Colors.White).Bold();
                                header.Cell().Element(CellStyle).Background(Colors.Blue.Darken2).Text("Nombre Completo").FontColor(Colors.White).Bold();
                                header.Cell().Element(CellStyle).Background(Colors.Blue.Darken2).Text("Email").FontColor(Colors.White).Bold();
                                header.Cell().Element(CellStyle).Background(Colors.Blue.Darken2).Text("Teléfono").FontColor(Colors.White).Bold();
                                header.Cell().Element(CellStyle).Background(Colors.Blue.Darken2).Text("Grado").FontColor(Colors.White).Bold();
                                header.Cell().Element(CellStyle).Background(Colors.Blue.Darken2).Text("Sección").FontColor(Colors.White).Bold();
                                header.Cell().Element(CellStyle).Background(Colors.Blue.Darken2).Text("Estado").FontColor(Colors.White).Bold();
                            });

                            // Datos
                            int contador = 1;
                            foreach (var matricula in matriculas)
                            {
                                var bgColor = contador % 2 == 0 ? Colors.Grey.Lighten4 : Colors.White;
                                
                                table.Cell().Element(CellStyle).Background(bgColor).AlignCenter().Text(contador.ToString());
                                table.Cell().Element(CellStyle).Background(bgColor).Text(matricula.Documento);
                                table.Cell().Element(CellStyle).Background(bgColor).Text(matricula.NombreCompleto);
                                table.Cell().Element(CellStyle).Background(bgColor).Text(matricula.Email ?? "-");
                                table.Cell().Element(CellStyle).Background(bgColor).Text(matricula.Telefono ?? "-");
                                table.Cell().Element(CellStyle).Background(bgColor).Text(matricula.Grado);
                                table.Cell().Element(CellStyle).Background(bgColor).AlignCenter().Text(matricula.Seccion);
                                table.Cell().Element(CellStyle).Background(bgColor).AlignCenter()
                                    .Text(matricula.Estado)
                                    .FontColor(matricula.Estado == "activo" ? Colors.Green.Darken2 : Colors.Orange.Darken2)
                                    .Bold();
                                
                                contador++;
                            }
                        });
                    });

                page.Footer()
                    .AlignCenter()
                    .Text(x =>
                    {
                        x.Span("Página ");
                        x.CurrentPageNumber();
                        x.Span(" de ");
                        x.TotalPages();
                    });
            });
        });

        return document.GeneratePdf();
    }

    private static IContainer CellStyle(IContainer container)
    {
        return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5);
    }
}
