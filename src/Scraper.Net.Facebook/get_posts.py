import json
import traceback, sys
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


class FacebookScraperException:
    type: str
    message: str
    stack_trace: str

    def __init__(self, exception: Exception):
        self.type = type(exception).__name__
        self.message = str(exception)
        self.stack_trace = get_stack_trace()


def get_stack_trace():
    exc = sys.exc_info()[0]
    stack = traceback.extract_stack()[:-1]  # last one would be full_stack()
    if exc is not None:  # i.e. an exception is present
        del stack[-1]       # remove call of full_stack, the printed exception
                            # will contain the caught exception caller instead
    return ''.join(traceback.format_list(stack))


def json_converter(obj):
    if isinstance(obj, datetime):
        return obj.__str__()
    if isinstance(obj, FacebookScraperException):
        return obj.__dict__


def main(args):
    try:
        request = GetPostsRequest(json.loads(args[0]))

        for post in get_facebook_posts(request):
            print(serialize(post))

    except Exception as e:
        error = FacebookScraperException(e)
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
