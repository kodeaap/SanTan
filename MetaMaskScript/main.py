from selenium import webdriver
from selenium.webdriver.support.ui import Select
from selenium.webdriver.common.by import By
from selenium.webdriver.support.ui import WebDriverWait
from selenium.webdriver.support import expected_conditions as EC
from selenium.webdriver.common.desired_capabilities import DesiredCapabilities
from selenium.webdriver.common.action_chains import ActionChains
from webdriver_manager.chrome import ChromeDriverManager
from selenium.webdriver.chrome.options import Options
from selenium.webdriver.common.keys import Keys
import random
import time
import os

def get_balance(driver):
	elm = WebDriverWait(driver, 15).until(EC.presence_of_element_located((By.XPATH, "//*[contains(@href,'/x-flip')]")))
	balance = float(elm.text.strip().replace('$',''))
	return balance

def main():

	options = webdriver.ChromeOptions()
	options.add_experimental_option('excludeSwitches', ['enable-logging'])
	options.add_extension('MetaMask.crx')
	caps = DesiredCapabilities().CHROME
	caps["pageLoadStrategy"] = "eager"
	driver = webdriver.Chrome(executable_path=ChromeDriverManager().install(),options=options,desired_capabilities = caps)
	driver.get('https://rollbit.com/x-flip')
	n = input('>>> Login and press fast flip and press enter: ')
	while True:
		try:
			data = open('output.txt','r').readlines()[0]
			data = data.split(',')
			bet_amount = data[0]
			side = data[1].strip()
			print(f'>>> Side: {side} | Amount: {bet_amount}')
			element = ''
			if side == '2':
				coin_element = "//button[span='Tails']"
			else:
				coin_element = "//button[span='Heads']"

			elm = WebDriverWait(driver, 15).until(EC.presence_of_element_located((By.XPATH, coin_element)))
			driver.execute_script("arguments[0].click();", elm)

			elm = WebDriverWait(driver, 15).until(EC.presence_of_element_located((By.XPATH, "//input[@type='text'][@autocomplete='off']")))
			for x in range(5):
				elm.send_keys(Keys.BACKSPACE)
			elm.send_keys(bet_amount)
			
			elm = WebDriverWait(driver, 15).until(EC.presence_of_element_located((By.XPATH, "//*[contains(text(), 'Flip Coin')]")))
			driver.execute_script("arguments[0].click();", elm)
			print('>>> Awaiting results')
			stat = ''
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
		except Exception as e:
			pass
	
main()