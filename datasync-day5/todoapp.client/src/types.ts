export interface Todo {
    id: string;
    title: string;
    completed: boolean;
}

export type ActionType =
  | { type: 'add'; title: string }
  | { type: 'remove'; id: string }
  | { type: 'edit'; todo: Todo }
  | { type: 'toggle-all'; checked: boolean }
  | { type: 'clear' }

export interface TodoProviderState {
  todos: Todo[]
  dispatch: React.Dispatch<ActionType>
}

export type VisibilityType = 'all' | 'active' | 'completed'

export interface VisibilityProviderState {
  visibility: VisibilityType
  setVisibility: React.Dispatch<React.SetStateAction<VisibilityType>>
}