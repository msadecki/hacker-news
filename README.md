# hacker-news repository
This is a recruitment task about using the Hacker News API to provide (top n) best stories.

## Endpoint
The repository contains a single solution with a single web application that implements a single controller with a single endpoint.
- a request URL example: https://localhost:7263/api/BestStories?topCount=15

## Remarks
To efficiently service large numbers of requests without risking overloading of the Hacker News API a simple memory cache is used (with hardcoded absolute expiration timeout equal 60 seconds, relative to now).

## Possible future enhancements or changes
It might be useful to implement the use of configuration variables to control memory cache management.