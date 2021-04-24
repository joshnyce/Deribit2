this version is still relatively simple, incomplete, and far from production-ready, but it includes some ideas for flexibility and architecture


todo:

use separate socket connections for each subscription, distribute load, add redundancy

listen to more events and log all activity (like subscriptions starting and stopping)

listen to heartbeat and add timer to try reconnecting until successful

inlcude quotes (in addition to trades) and any other types of events that are of interest

inlcude symbols other than futures, and generally explore API to figure out how to get more data

way to request replays from the API in case of outages (looks like sequence number can be used to specify ranges of historical data)

way to add and remove subscriptions dynamically, if that's desirable

use a windows service or need keep-alive system

add more logging destinations