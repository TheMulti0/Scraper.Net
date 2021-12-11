import json
import sys
from itertools import cycle
from random import shuffle
from typing import Optional

from facebook_scraper import get_page_info, set_proxy, set_cookies
from facebook_scraper.exceptions import InvalidCookies


class GetPageInfoRequest:
    user_id: str
    proxy: Optional[str]
    timeout: int
    cookies_filenames: list[str]

    def __init__(self, new_dict):
        self.__dict__.update(new_dict)


class Error:
    type: str
    message: str

    def __init__(self, exception: Exception):
        self.type = type(exception).__name__
        self.message = str(exception)


def json_converter(obj):
    if isinstance(obj, Error):
        return obj.__dict__


def main(args):
    try:
        request = GetPageInfoRequest(json.loads(args[0]))

        shuffle(request.cookies_filenames)
        cookies = cycle(request.cookies_filenames)
        set_cookie(cookies)

        page_info = get_facebook_page_info(request)
        print(serialize(page_info))

    except Exception as e:
        error = Error(e)
        print(serialize(error))


def get_facebook_page_info(request: GetPageInfoRequest):
    if request.proxy is not None:
        set_proxy(request.proxy)

    return get_page_info(
        request.user_id,
        timeout=request.timeout)


def set_cookie(cookies, initial=None):
    try:
        cookie = next(cookies)

        if initial == cookie:
            raise StopIteration

        print(f'Trying cookies from file {cookie}')

        try:
            set_cookies(cookie)
        except (InvalidCookies, FileNotFoundError) as e:
            print(f'Failed to use cookies from file {cookie}:')
            print(f'{type(e).__name__}: {e}')

            set_cookie(
                cookies,
                initial=cookie if initial is None else initial)

    except StopIteration:
        print("Not using cookies")
        return


def serialize(obj):
    return json.dumps(obj, indent=2, default=json_converter)


main(sys.argv[1:])
