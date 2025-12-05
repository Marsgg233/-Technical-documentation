using Microsoft.Win32;
using RAGSystem.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;

namespace RAGSystem
{
    public partial class MainWindow : Window
    {
        private VectorStore vectorStore = new VectorStore();
        private string vectorsFile = "vectors.json";
        private List<string> keywords = new List<string>();

        public MainWindow()
        {
            InitializeComponent();
            LoadVectors();
            InitializeKeywords();
        }

        private void InitializeKeywords()
        {
            keywords = new List<string>
            {
                "пепел", "игра", "система", "механика", "крафт",
                "выживание", "ресурс", "враг", "способность", "оружие",
                "броня", "персонаж", "мир", "огнь", "пламя",
                "требование", "функция", "пользователь", "интерфейс",
                "процесс", "разработка", "технический", "дизайн",
                "геймплей", "сюжет", "персонажи", "уровень", "сложность"
            };
        }

        private void LoadVectors()
        {
            if (File.Exists(vectorsFile))
            {
                try
                {
                    string json = File.ReadAllText(vectorsFile);
                    var loadedStore = JsonSerializer.Deserialize<VectorStore>(json);

                    if (loadedStore != null)
                    {
                        vectorStore = loadedStore;
                        tbStatus.Text = $"Загружено {vectorStore.Chunks.Count} частей";
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка загрузки векторов: {ex.Message}");
                }
            }
        }

        private void SaveVectors()
        {
            if (vectorStore.Chunks.Count == 0)
            {
                MessageBox.Show("Нет частей для сохранения");
                return;
            }

            try
            {
                string json = JsonSerializer.Serialize(vectorStore, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(vectorsFile, json);
                tbStatus.Text = $"Векторы сохранены в {vectorsFile}";
                MessageBox.Show($"Сохранено в файл: {vectorsFile}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}");
            }
        }

        private float[] CreateSimpleVector(string text)
        {
            float[] vector = new float[keywords.Count];
            string lowerText = text.ToLower();

            for (int i = 0; i < keywords.Count; i++)
            {
                if (lowerText.Contains(keywords[i]))
                {
                    vector[i] = 1.0f;
                }
            }

            return vector;
        }

        private double CalculateCosineSimilarity(float[] vec1, float[] vec2)
        {
            double dotProduct = 0;
            double magnitude1 = 0;
            double magnitude2 = 0;

            for (int i = 0; i < vec1.Length; i++)
            {
                dotProduct += vec1[i] * vec2[i];
                magnitude1 += vec1[i] * vec1[i];
                magnitude2 += vec2[i] * vec2[i];
            }

            if (magnitude1 == 0 || magnitude2 == 0)
                return 0;

            return dotProduct / (Math.Sqrt(magnitude1) * Math.Sqrt(magnitude2));
        }

        private List<string> SplitTextIntoChunks(string text)
        {
            var chunks = new List<string>();

            string[] paragraphs = text.Split(new[] { "\r\n\r\n", "\n\n" }, StringSplitOptions.RemoveEmptyEntries);

            var currentChunk = new StringBuilder();

            foreach (string paragraph in paragraphs)
            {
                if (currentChunk.Length + paragraph.Length > 500 && currentChunk.Length > 0)
                {
                    chunks.Add(currentChunk.ToString());
                    currentChunk.Clear();
                }

                currentChunk.AppendLine(paragraph);
            }

            if (currentChunk.Length > 0)
            {
                chunks.Add(currentChunk.ToString());
            }

            return chunks;
        }

        // КНОПКА: Загрузить ТЗ
        private void BtnLoad_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Все файлы (*.*)|*.*|Текстовые файлы (*.txt)|*.txt|Markdown (*.md)|*.md";

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    string content = File.ReadAllText(dialog.FileName);
                    txtTZ.Text = content;
                    tbStatus.Text = $"Загружен файл: {dialog.FileName}";
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка загрузки файла: {ex.Message}");
                }
            }
        }

        // КНОПКА: Разбить на части
        private void BtnSplit_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtTZ.Text))
            {
                MessageBox.Show("Загрузите ТЗ сначала");
                return;
            }

            vectorStore.Chunks.Clear();
            string text = txtTZ.Text;

            var chunks = SplitTextIntoChunks(text);

            int id = 1;
            foreach (var chunkText in chunks)
            {
                var chunk = new Chunk();
                chunk.Id = id++;
                chunk.Text = chunkText;
                chunk.Vector = CreateSimpleVector(chunkText);
                chunk.Similarity = 0;

                vectorStore.Chunks.Add(chunk);
            }

            tbStatus.Text = $"Создано {vectorStore.Chunks.Count} частей с векторами";
            MessageBox.Show($"Текст разбит на {vectorStore.Chunks.Count} частей. Векторы созданы.");
        }

        // КНОПКА: Сохранить векторы
        private void BtnSaveVectors_Click(object sender, RoutedEventArgs e)
        {
            SaveVectors();
        }

        // КНОПКА: Найти похожие
        private void BtnFind_Click(object sender, RoutedEventArgs e)
        {
            if (vectorStore.Chunks.Count == 0)
            {
                MessageBox.Show("Сначала разбейте текст");
                return;
            }

            string query = txtQuery.Text;
            if (string.IsNullOrEmpty(query))
            {
                MessageBox.Show("Введите запрос для поиска");
                return;
            }

            float[] queryVector = CreateSimpleVector(query);

            foreach (var chunk in vectorStore.Chunks)
            {
                chunk.Similarity = CalculateCosineSimilarity(queryVector, chunk.Vector);
            }

            var results = vectorStore.Chunks
                .OrderByDescending(c => c.Similarity)
                .Take(5)
                .ToList();

            dgResults.ItemsSource = results;

            tbStatus.Text = $"Найдено {results.Count} похожих частей";

            txtAnswer.Text = $"Результаты поиска для: \"{query}\"\n\n";
            foreach (var chunk in results)
            {
                txtAnswer.Text += $"=== Часть {chunk.Id} (сходство: {chunk.Similarity:P1}) ===\n";
                txtAnswer.Text += chunk.Text + "\n\n";
            }
        }

        // КНОПКА: Сгенерировать
        private void BtnGenerate_Click(object sender, RoutedEventArgs e)
        {
            if (vectorStore.Chunks.Count == 0)
            {
                MessageBox.Show("Сначала разбейте текст");
                return;
            }

            string query = txtQuery.Text;
            if (string.IsNullOrEmpty(query))
            {
                MessageBox.Show("Введите запрос");
                return;
            }

            float[] queryVector = CreateSimpleVector(query);

            foreach (var chunk in vectorStore.Chunks)
            {
                chunk.Similarity = CalculateCosineSimilarity(queryVector, chunk.Vector);
            }

            var topChunks = vectorStore.Chunks
                .OrderByDescending(c => c.Similarity)
                .Take(3)
                .ToList();

            StringBuilder answer = new StringBuilder();
            answer.AppendLine($"Ответ на вопрос: \"{query}\"");
            answer.AppendLine("Основано на техническом задании:");
            answer.AppendLine();

            foreach (var chunk in topChunks)
            {
                answer.AppendLine(chunk.Text);
                answer.AppendLine();
            }

            answer.AppendLine("---");
            answer.AppendLine("Это информация из технического задания игры.");

            txtAnswer.Text = answer.ToString();
        }
    }
}