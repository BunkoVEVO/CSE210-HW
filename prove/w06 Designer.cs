import random
import os

class Word:
    def __init__(self, text):
        self.text = text
        self.is_hidden = False

    def hide(self):
        self.is_hidden = True

    def __str__(self):
        return '_____' if self.is_hidden else self.text

class Reference:
    def __init__(self, book, chapter, start_verse, end_verse=None):
        self.book = book
        self.chapter = chapter
        self.start_verse = start_verse
        self.end_verse = end_verse

    def __str__(self):
        if self.end_verse:
            return f"{self.book} {self.chapter}:{self.start_verse}-{self.end_verse}"
        else:
            return f"{self.book} {self.chapter}:{self.start_verse}"

class Scripture:
    def __init__(self, reference, text):
        self.reference = reference
        self.words = [Word(word) for word in text.split()]

    def hide_words(self):
        available_words = [word for word in self.words if not word.is_hidden]
        if available_words:
            word_to_hide = random.choice(available_words)
            word_to_hide.hide()

    def is_fully_hidden(self):
        return all(word.is_hidden for word in self.words)

    def __str__(self):
        return f"{self.reference}\n" + ' '.join(str(word) for word in self.words)

class Program:
    @staticmethod
    def clear_console():
        os.system('cls' if os.name == 'nt' else 'clear')

    @staticmethod
    def display_scripture(scripture):
        Program.clear_console()
        print(scripture)

    @staticmethod
    def prompt_user():
        return input("Press Enter to hide words or type 'quit' to exit: ")

    def main(self):
        reference = Reference("Proverbs", 3, 5, 6)
        text = "Trust in the Lord with all your heart and lean not on your own understanding; in all your ways submit to him, and he will make your paths straight."
        scripture = Scripture(reference, text)

        while not scripture.is_fully_hidden():
            Program.display_scripture(scripture)
            user_input = Program.prompt_user()
            if user_input.lower() == 'quit':
                break
            scripture.hide_words()

        Program.display_scripture(scripture)
        print("All words are hidden. Program ended.")

if __name__ == "__main__":
    program = Program()
    program.main()
