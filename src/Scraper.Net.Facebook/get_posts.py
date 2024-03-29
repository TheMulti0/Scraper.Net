﻿import json
import sys
import traceback
import warnings
from datetime import datetime
from itertools import cycle
from random import shuffle
from typing import Optional, List

from facebook_scraper import get_posts, set_proxy, set_cookies
from facebook_scraper.exceptions import InvalidCookies


class GetPostsRequest:
    user_id: str
    pages: int
    posts_per_page: int
    proxy: Optional[str]
    timeout: int
    cookies_filenames: List[str]

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
        del stack[-1]  # remove call of full_stack, the printed exception
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

        shuffle(request.cookies_filenames)
        cookies = cycle(request.cookies_filenames)
        set_cookie(cookies)

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
        timeout=request.timeout,
        options={"posts_per_page": request.posts_per_page})


def set_cookie(cookies: List[str], initial=None):
    try:
        cookie = next(cookies)

        if initial == cookie:
            raise StopIteration

        warnings.warn(f'Trying cookies from file {cookie}')

        try:
            set_cookies(cookie)
        except (InvalidCookies, FileNotFoundError) as e:
            warnings.warn(f'Failed to use cookies from file {cookie}:')
            warnings.warn(f'{type(e).__name__}: {e}')

            set_cookie(
                cookies,
                initial=cookie if initial is None else initial)

    except StopIteration:
        warnings.warn("Not using cookies")
        return


def serialize(obj):
    return json.dumps(obj, indent=2, default=json_converter)


main(sys.argv[1:])
