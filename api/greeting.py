from flask import Flask, request, jsonify
import json

import gpt2

app = Flask(__name__)

scenarios = dict()
with open('aurora_contexts.json') as json_file:
    scenarios = json.load(json_file)

@app.route('/getmsg/', methods=['GET'])
def respond():
    # Retrieve the name from the url parameter /getmsg/?mode=
    gpt_input = request.args.get("input", None)
    mode = request.args.get("mode", "introduction")

    # For debugging
    print(f"Received: {gpt_input}")

    response = {}

    if not gpt_input:
        gpt_input = scenarios[mode]

    response["MESSAGE"] = gpt2.generate_text(gpt_input, max_length=(len(gpt_input.split()) + 50))
    scenarios[mode] = response["MESSAGE"]

    # Return the response in json format
    return jsonify(response)

@app.route('/')
def index():
    # A welcome message to test our server
    return "<h1>Welcome to Great Path API!</h1>"


if __name__ == '__main__':
    # Threaded option to enable multiple instances for multiple user access support
    app.run(threaded=True, port=5000)