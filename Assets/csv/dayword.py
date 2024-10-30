import csv

def read_text_file(file_path):
    # 读取文本文件并提取大总结和其后的单词
    summaries = []
    current_summary = None
    words_under_summary = []

    with open(file_path, 'r', encoding='utf-8') as file:
        for line in file:
            words = line.strip().split()  # 按空格分割行中的单词
            for word in words:
                if '_' not in word:  # 识别大总结
                    if current_summary:
                        summaries.append((current_summary, words_under_summary))
                    current_summary = word
                    words_under_summary = []
                else:
                    words_under_summary.append(word)
        if current_summary:  # 将最后一个大总结添加到列表
            summaries.append((current_summary, words_under_summary))

    return summaries

def write_day_csv(summaries, day_file_path):
    # 写入day.csv，每一行是一个大总结
    with open(day_file_path, 'a', newline='', encoding='utf-8') as csvfile:
        writer = csv.writer(csvfile)
        writer.writerow([summary for summary, _ in summaries])

def write_big2small_csv(summaries, big2small_file_path):
    # 写入big2small.csv，每一行是一个大总结及其对应的单词
    with open(big2small_file_path, 'a', newline='', encoding='utf-8') as csvfile:
        writer = csv.writer(csvfile)
        for summary, words in summaries:
            row = [summary] + words
            writer.writerow(row)

# 文件路径
text_file_path = 'test.txt'
day_file_path = 'day.csv'
big2small_file_path = 'big.csv'

# 执行读取和写入操作
summaries = read_text_file(text_file_path)
write_day_csv(summaries, day_file_path)
write_big2small_csv(summaries, big2small_file_path)

