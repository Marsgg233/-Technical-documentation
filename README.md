Марсель Габбасов
ИТ11.24.3
GabbasovMR24@spb.ithub.ru

# Инструкция по клонированию репозитория:

 1. Откройте терминал
 2. Перейдите в нужную папку:
```
cd путь/к/вашей/папке
```

 3. Склонируйте репозиторий:
```
git clone https://github.com/ваш-username/название-репозитория.git
```

 4. Перейдите в папку проекта:
```
cd название-репозитория
```
# Инструкция по загрузке коммита:

 1. Добавьте изменённые файлы:
```
git add .
```
 *Или*
 
для конкретного файла:
```
git add имя_файла.md
```
 2. Создайте коммит:
```
git commit -m "Описание внесённых изменений"
```
 3. Загрузите коммит в репозиторий:
```
git push origin main
```
*Или*

```
git push origin master
```
# Дополнительные команды:

 Проверить статус:
```
git status
```

 Просмотреть историю коммитов:
```
git log
```

 Обновить локальный репозиторий:
 ```
git pull origin main
```

<img width="268" height="240" alt="image" src="https://github.com/user-attachments/assets/35eb2406-2d61-4db8-b296-6090c6e887b7" />

# Установка Git (если не установлен):

 Для Ubuntu/Debian:
```
sudo apt update
```
```
sudo apt install git
```
 Для macOS:
```
brew install git
```
 Для Windows - скачайте с сайта:
 ```
https://gitforwindows.org/
 ```
 <img width="792" height="613" alt="image" src="https://github.com/user-attachments/assets/0d7d1638-8fdb-4804-9c49-ad2fecd1c2f5" />

