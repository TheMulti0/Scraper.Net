import json
import sys
from datetime import datetime
from typing import Optional

from facebook_scraper import get_posts, set_proxy


class GetPostsRequest:
    user_id: str
    pages: int
    proxy: Optional[str]
    timeout: int
    cookies_filename: Optional[str]

    def __init__(self, new_dict):
        self.__dict__.update(new_dict)


class Error:
    type: str
    message: str

    def __init__(self, exception: Exception):
        self.type = type(exception).__name__
        self.message = str(exception)


def json_converter(obj):
    if isinstance(obj, datetime):
        return obj.__str__()
    if isinstance(obj, Error):
        return obj.__dict__


def main(args):
    request = GetPostsRequest(json.loads(args[0]))

    try:
        for post in get_facebook_posts(request):
            print(serialize(post))

    except Exception as e:
        error = Error(e)
        print(serialize(error))


def get_facebook_posts(request: GetPostsRequest):
    if request.proxy is not None:
        set_proxy(request.proxy)

    return get_posts(
        request.user_id,
        pages=request.pages,
        cookies=request.cookies_filename,
        timeout=request.timeout)


def serialize(obj):
    return json.dumps(obj, indent=2, default=json_converter)


if __name__ == "__main__":
    main(sys.argv[1:])
