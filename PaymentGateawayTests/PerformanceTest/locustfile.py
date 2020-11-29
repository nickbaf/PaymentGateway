# This locust test script example will simulate a user 
# browsing the Locust documentation on https://docs.locust.io

import random
from locust import HttpUser, between, task
from pyquery import PyQuery


class AwesomeUser(HttpUser):
    host = "https://localhost:8080/"
    
    # we assume someone who is browsing the Locust docs, 
    # generally has a quite long waiting time (between 
    # 10 and 600 seconds), since there's a bunch of text 
    # on each page
    wait_time = between(10, 600)
    
    def on_start(self):
        # start by waiting so that the simulated users 
        # won't all arrive at the same time
        self.wait()
        # assume all users arrive at the index page
        self.authorizePayment()
        self.urls_on_current_page = self.toc_urls
    
    @task(10)
    def authorizePayment(self):
        response = self.client.post("/api/authorize", {
  "card": {
    "number": "5186124094923094",
    "expirationMonthAndYear": {
      "month": "11",
      "year": "24"
    },
    "cvv": "123"
  },
  "money": {
    "amount": 5000,
    "currency": "JPY"
  }
})
        print("Response status code:", response.status_code)
        print("Response text:", response.text)
    
    