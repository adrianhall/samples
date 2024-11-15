import { clsx } from 'ts-clsx';
import { Todo } from '../types';
import { useTodos } from './TodoProvider.tsx';
import { useEffect, useRef, useState } from 'react';

export default function TodoItem({ todo }: { todo: Todo }) {
    const { dispatch } = useTodos();
    const [isEditing, setIsEditing] = useState(false);
    const [editedTitle, setEditedTitle] = useState(todo.title);
    const inputRef = useRef<HTMLInputElement>(null);

    function handleEdit() {
        const title = editedTitle.trim()
        if (!title) {
            dispatch({ type: 'remove', id: todo.id });
        } else {
            dispatch({ type: 'edit', todo: { ...todo, title } });
        }
        setIsEditing(false);
    }

    function handleCancelEdit() {
        setEditedTitle(todo.title);
        setIsEditing(false);
    }

    useEffect(() => {
        if (isEditing && inputRef.current) {
            inputRef.current.focus();
        }
    }, [isEditing]);

    return (
        <li
            className={clsx('todo', {
                completed: todo.completed,
                editing: isEditing,
            })}
        >
            <div className="view">
                <input
                    type="checkbox"
                    className="toggle"
                    checked={todo.completed}
                    onChange={(e) => {
                        dispatch({
                            type: 'edit',
                            todo: { ...todo, completed: e.target.checked },
                        })
                    }}
                />
                <label onDoubleClick={() => setIsEditing(true)}>{todo.title}</label>
                <button className="destroy" onClick={() => dispatch({ type: 'remove', id: todo.id })}></button>
            </div>
            {isEditing && (
                <input
                    ref={inputRef}
                    className="edit"
                    value={editedTitle}
                    onChange={(e) => setEditedTitle(e.target.value)}
                    onBlur={handleEdit}
                    onKeyUp={(e) => {
                        if (e.key === 'Enter') handleEdit()
                        else if (e.key === 'Escape') handleCancelEdit()
                    }}
                />
            )}
        </li>
    );
}