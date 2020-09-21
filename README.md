# GameSessionFeedback Service

## REST API Documentation
### Get Game Sessions Feedback
**URL** : `/api/feedback/` or `/api/feedback/:rate/`

**URL Parameters** : `rate=[integer][optional][range 1-5]` where `rate` is the rating of the feedback for filtering.

**Method** : `GET`

## Success Response

**Condition** : If rate not out of range.

**Code** : `200 OK`

**Content example**

```json
[
    {
        "id": "5f689766247f6a944c63d34c",
        "user": "test",
        "session": "2",
        "rating": 5,
        "comment": "this is a comment",
        "feedbackDate": "2020-09-21 12:07:02"    
    },
    {
        "id": "5f689766247f6a944c63d34c",
        "user": "test",
        "session": "2",
        "rating": 5,
        "comment": "this is a comment",
        "feedbackDate": "2020-09-21 12:07:02"
    }
]
```

## Error Responses

**Condition** : If `rate` out of range.

**Code** : `400 BAD REQUEST`

**Content** 
```json
{
     "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
     "title": "Bad Request",
     "status": 400,
     "traceId": "|eb553ca3-42299e0e7063b2e6."
}
```

### Create Game Sessions Feedback
**URL** : `/api/feedback/` or `/api/feedback/:sessionId/`

**URL Parameters** : `sessionId=[string][required]` where `sessionId` is the last session id of the player.

**Headers** : `Ubi-UserId=[string][required]` where `Ubi-UserId` is the user id identifier

**Method** : `POST`

**Data** : 
```
"rating": [required][integer][range 1-5],
"comment": [required][string]
```

## Success Response

**Condition** : If user didn't already inserted feedback for its sessionId and rate not out of range.

**Code** : `201 Created`

**Body example**

```json
```

## Error Responses

**Condition** : If `rate` out of range.

**Code** : `400 BAD REQUEST`

**Content** 
```json
{
    "errors": {
        "Rate": [
            "The field Rate must be between 1 and 5."
        ]
    },
    "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
    "title": "One or more validation errors occurred.",
    "status": 400,
    "traceId": "|eb553caf-42299e0e7063b2e6."
}
```
-------------------------

**Condition** : If header `Ubi-UserId` missing or empty.

**Code** : `400 BAD REQUEST`

**Content** 
```json
{
    "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
    "title": "Bad Request",
    "status": 400,
    "traceId": "|eb553cb1-42299e0e7063b2e6."
}
```
-------------------------

**Condition** : If a feedback already exists for the tuple(userId,sessionId).

**Code** : `409 CONFLICT`

**Content** 
```json
{
    "type": "https://tools.ietf.org/html/rfc7231#section-6.5.8",
    "title": "Conflict",
    "status": 409,
    "traceId": "|eb553cb2-42299e0e7063b2e6."
}
```

## Running tests
To run the tests use the following command with dotnet cli. It will exectue unit tests and integration tests together.
```
dotnet test
```
