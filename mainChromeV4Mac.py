from selenium import webdriver
from selenium.webdriver.common.by import By
from selenium.webdriver.support.ui import WebDriverWait
from selenium.webdriver.support import expected_conditions as EC
from webdriver_manager.firefox import GeckoDriverManager
from selenium.webdriver.firefox.options import Options
from selenium.webdriver.common.keys import Keys
import random
import time
import os

def get_balance(driver):
    elm = WebDriverWait(driver, 15).until(EC.presence_of_element_located((By.XPATH, "//*[contains(@href,'/x-flip')]")))
    balance = float(elm.text.strip().replace('$',''))
    return balance

def main():
    profile_directory = "/Users/ramiemera/Library/Application Support/Firefox/Profiles/8nz95ahw.default-release-1678506186244"
    options = Options()
    options.profile = profile_directory
    driver = webdriver.Firefox(executable_path=GeckoDriverManager().install(), options=options)
    driver.get('https://rollbit.com/')
    n = input('>>> Login and press fast flip and press enter: ')
    driver.get('https://rollbit.com/x-flip')
    counter = 0
    status = 'Unknown'
    while True:
        try:
            data = open('output.txt','r').readlines()[0]
            data = data.split(',')
            bet_amount = data[0]
            side = data[1].strip()
            stat = ''
            print(f'>>> Side: {side} | Amount: {bet_amount} | Status: {stat}')
            element = ''
            if side == '2':
                coin_element = "//button[span='Tails']"
            else:
                coin_element = "//button[span='Heads']"

            # Wait for random time between 0.5 and 2 seconds
            wait_time = random.uniform(0.5, 2)
            #print(f'>>> Waiting for {wait_time:.2f} seconds')
            time.sleep(wait_time)

            # Find all elements containing text that starts with "$"
            dollar_elems = driver.find_elements_by_xpath(".//*[starts-with(text(), '$')]")

            if len(dollar_elems) > 5:
                value_4 = float(dollar_elems[4].text.replace(",", "").replace("$", ""))
                value_5 = float(dollar_elems[5].text.replace(",", "").replace("$", ""))
                if value_4 >= value_5:
                    print('Reached Required Limit...Stopping Program')
                    quit()

            # Iterate over the dollar elements and print each one along with its index
            #for i, dollar_elem in enumerate(dollar_elems):
            #    print(f"{i + 1}. {dollar_elem.text}")

            elm = WebDriverWait(driver, 15).until(EC.presence_of_element_located((By.XPATH, coin_element)))
            driver.execute_script("arguments[0].click();", elm)

            elm = WebDriverWait(driver, 15).until(EC.presence_of_element_located((By.XPATH, "//input[@type='text'][@autocomplete='off']")))
            for x in range(5):
                elm.send_keys(Keys.BACKSPACE)
            elm.send_keys(bet_amount)

            elm = WebDriverWait(driver, 15).until(EC.presence_of_element_located((By.XPATH, "//*[contains(text(), 'Flip Coin')]")))
            driver.execute_script("arguments[0].click();", elm)
            # print('>>> Awaiting results')
            
            while True:
                try:
                    elm = WebDriverWait(driver, 1).until(EC.presence_of_element_located((By.XPATH, "//*[contains(text(), 'You Won ')]")))
                    stat = 'won'
                    break
                except:
                    try:
                        elm = WebDriverWait(driver, 1).until(EC.presence_of_element_located((By.XPATH, "//*[contains(text(), 'You Lost')]")))
                        stat = 'lost'
                        break
                    except:
                        pass
            out = ''
            if stat == 'lost':
                if side == '1':
                    out = '2'
                else:
                    out = '1'
            else:
                out = side
            fl = open('input.txt','w')
            fl.write(out)
            fl.close()
            os.remove('output.txt')
            print('>>> Data written in input.txt | output.txt is deleted')
            counter += 1
            if counter % 2000 == 0:
                print('>>> Waiting for an hour...')
                time.sleep(3600)
        except Exception as e:
            pass

    driver.quit()

main()
