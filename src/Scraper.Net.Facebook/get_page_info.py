import json
import sys
from typing import Optional

from facebook_scraper import get_page_info, set_proxy


class GetPageInfoRequest:
    user_id: str
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
    if isinstance(obj, Error):
        return obj.__dict__


def main(args):
    request = GetPageInfoRequest(json.loads(args[0]))

    try:
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
        cookies=request.cookies_filename,
        timeout=request.timeout)


def serialize(obj):
    return json.dumps(obj, indent=2, default=json_converter)


if __name__ == "__main__":
    main(sys.argv[1:])
