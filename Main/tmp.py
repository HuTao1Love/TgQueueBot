@dp.message_handler(commands=["delaystartq"])
async def delay_create_queue(message: types.Message):
    if message.from_user.id not in CAN_CREATE_QUEUES:
        await message.answer("Ты не можешь создать очередь")
        return
    if len(message.text.split()) < 2:
        await message.answer("Usage: /delaystartq t=<await time> <delay, default=10> <qname>")
        return
    time = 10
    if message.text.split()[1].startswith('t='):
        time = int(message.text.split()[1][2:])
        if time < 0:
            await message.answer("Задержка не может быть отрицательной")
            return
        message.text = message.text.replace(message.text.split()[1] + " ", "")

    SLEEP_TIME = 10
    start_time = time

    callback_query = await message.answer(f"Очередь запустится через {time} сек")
    while True:
        await asyncio.sleep(min(SLEEP_TIME, time))
        time -= SLEEP_TIME
        if time <= 0:
            break
        await bot.edit_message_text(message_id=callback_query.message_id, chat_id=callback_query.chat.id,
                                    text=f"Очередь запустится через {time} (start={start_time}) сек")
    await create_queue(message)
    await bot.edit_message_text(message_id=callback_query.message_id, chat_id=callback_query.chat.id,
                                text=f"Очередь запущена!")


@dp.message_handler(commands=["listq"])
async def queue_list(message: types.Message):
    if message.from_user.id != BOT_CREATOR:
        await message.answer("Only for Hu Tao")
        return

    msg = "Очереди:\n"
    for (chat_id, queues_list) in queues.items():
        msg += f"{CHAT_IDS.get(chat_id, chat_id)}:\n"
        for name in queues_list.keys():
            msg += f"{name}\n"
        msg += "\n"

    await message.answer(text=msg)


@dp.message_handler(commands=["delete"])
async def delete(message: types.Message):
    if message.from_user.id != BOT_CREATOR:
        await message.answer("Only for Hu Tao")
        return

    for i in list(queues[message.chat.id].keys()):
        del queues[message.chat.id][i]
    await message.answer(text="Done")


@dp.message_handler(commands=["deleteall"])
async def delete_all(message: types.Message):
    if message.from_user.id != BOT_CREATOR:
        await message.answer("Only for Hu Tao")
        return

    for i in list(queues.keys()):
        del queues[i]
    await message.answer(text="Done")


@dp.message_handler(commands=["shutdown", "exit"])
async def shutdown(message: types.Message):
    if message.from_user.id != BOT_CREATOR:
        await message.answer("Only for Hu Tao")
        return

    dp.stop_polling()
    pickle.dump(queues, open("queues.txt", "wb"))
    exit()


@dp.callback_query_handler(lambda c: c.data and c.data.startswith('key'))
async def insert_in_queue(callback_query: types.CallbackQuery):
    _, code, qname = callback_query.data.split("/")
    code = int(code)
    user_id = callback_query.from_user.id
    name = f"{callback_query.from_user.full_name} (@{callback_query.from_user.username})"
    text, code = queues[callback_query.message.chat.id][qname].set(
        code, user_id, name)
    await bot.answer_callback_query(callback_query.id, text=text)
    if code:
        await asyncio.sleep(0.1)
        await bot.edit_message_text(message_id=callback_query.message.message_id,
                                    chat_id=callback_query.message.chat.id,
                                    text=f"{qname}:\n{queues[callback_query.message.chat.id][qname].get_print()} \n(Generated by {CAN_CREATE_QUEUES[queues[callback_query.message.chat.id][qname].creator].description})",
                                    reply_markup=queues[callback_query.message.chat.id][qname].get_keyboard())


@dp.callback_query_handler(lambda c: c.data and c.data.startswith("stop"))
async def delete_queue(callback_query: types.CallbackQuery):
    _, qname = callback_query.data.split("/")
    if qname not in queues[callback_query.message.chat.id].keys():
        await bot.edit_message_text(message_id=callback_query.message.message_id,
                                    chat_id=callback_query.message.chat.id,
                                    text=f"{qname} (stopped)")
        return

    if callback_query.from_user.id != queues[callback_query.message.chat.id][qname].creator and callback_query.from_user.id != BOT_CREATOR:
        await bot.answer_callback_query(callback_query.id, text="Это может сделать только создатель очереди")
        return

    await bot.edit_message_text(message_id=callback_query.message.message_id, chat_id=callback_query.message.chat.id,
                                text=f"{qname} (stopped):\n{queues[callback_query.message.chat.id][qname].get_print(full=False)}")
    del queues[callback_query.message.chat.id][qname]


@dp.callback_query_handler(lambda c: c.data and c.data.startswith('reset'))
async def reset_queue(callback_query: types.CallbackQuery):
    _, qname = callback_query.data.split("/")
    if callback_query.from_user.id != queues[callback_query.message.chat.id][qname].creator and callback_query.from_user.id != BOT_CREATOR:
        await bot.answer_callback_query(callback_query.id, text="Это может сделать только создатель очереди")
        return

    is_modified = queues[callback_query.message.chat.id][qname].reset()
    if is_modified:
        await bot.edit_message_text(message_id=callback_query.message.message_id,
                                    chat_id=callback_query.message.chat.id,
                                    text=f"{qname}:\n{queues[callback_query.message.chat.id][qname].get_print()} \n(Generated by {CAN_CREATE_QUEUES[queues[callback_query.message.chat.id][qname].creator].description})",
                                    reply_markup=queues[callback_query.message.chat.id][qname].get_keyboard())
    else:
        await bot.answer_callback_query(callback_query.id, text="Очередь и была пуста")