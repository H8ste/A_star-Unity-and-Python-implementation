#
#   Hello World server in Python
#   Binds REP socket to tcp://*:5555
#   Expects b"Hello" from client, replies with b"World"
#

import time
import zmq  # pylint: disable=import-error

print("Server starting...")
context = zmq.Context()
socket = context.socket(zmq.REP)
socket.bind("tcp://*:5555")

print("Server started");

while True:
    #  Wait for next request from client
    message = socket.recv()
    print("Recievede mes")
    print(message)
    print(",,,,")
    msgType = message.decode().split(':')
    print(msgType)
    if (msgType[0] == "0"):
        _allTiles = msgType[1].split(';')
        print("Printing all tiles taken from server")
        print(_allTiles)

        #  In the real world usage, you just need to replace time.sleep() with
        #  whatever work you want python to do.
        time.sleep(1)

        #  Send reply back to client
        #  In the real world usage, after you finish your work, send your output here
    socket.send(b"Hello Client!")

    # elif msgType[0] == 1:
    #     pass

    # if message received contains: "ALL TILES":

    # if message recieve contains: "PLAYER CLICKS":
