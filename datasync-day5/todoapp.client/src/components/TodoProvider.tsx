import {
    ReactNode,
    createContext,
    useContext,
    useEffect,
    useReducer,
    useState
} from 'react';
import {
    ActionType,
    Todo,
    TodoProviderState,
    VisibilityProviderState,
    VisibilityType
} from '../types';

const STORAGE_KEY = 'datasync-todomvc';

const TodosContext = createContext<TodoProviderState>({
    todos: [],
    dispatch: () => null,
});

const VisibilityContext = createContext<VisibilityProviderState>({
    visibility: 'all',
    setVisibility: () => null,
});

function todosReducer(todos: Todo[], action: ActionType) {
    switch (action.type) {
        case 'add':
            return [...todos, { id: crypto.randomUUID(), title: action.title, completed: false }]
        case 'remove':
            return todos.filter((todo) => todo.id !== action.id)
        case 'edit':
            return todos.map((todo) => (todo.id === action.todo.id ? action.todo : todo))
        case 'toggle-all':
            return todos.map((todo) => ({ ...todo, completed: action.checked }))
        case 'clear':
            return todos.filter((todo) => !todo.completed)
        default:
            return todos
    }
}

export default function TodoProvider({ children }: { children: ReactNode }) {
    const [todos, dispatch] = useReducer(todosReducer, null, () => JSON.parse(localStorage.getItem(STORAGE_KEY) || '[]'))
    const [visibility, setVisibility] = useState<VisibilityType>('all')

    useEffect(() => {
        localStorage.setItem(STORAGE_KEY, JSON.stringify(todos))
    }, [todos])

    return (
        <TodosContext.Provider value={{ todos, dispatch }}>
            <VisibilityContext.Provider value={{ visibility, setVisibility }}>{children}</VisibilityContext.Provider>
        </TodosContext.Provider>
    )
}

export function useTodos() {
    return useContext(TodosContext)
}

export function useVisibility() {
    return useContext(VisibilityContext)
}