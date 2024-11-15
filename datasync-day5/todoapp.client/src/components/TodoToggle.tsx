import { useTodos } from './TodoProvider.tsx';

export default function TodoToggle() {
  const { todos, dispatch } = useTodos();

  return (
    <>
      <input
        id="toggle-all"
        className="toggle-all"
        type="checkbox"
        checked={todos.every((todo) => todo.completed)}
        onChange={(e) => {
          dispatch({ type: 'toggle-all', checked: e.target.checked })
        }}
      />
      <label htmlFor="toggle-all">Mark all as completed</label>
    </>
  );
}